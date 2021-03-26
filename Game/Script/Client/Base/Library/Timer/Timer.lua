--无序定时器，单位秒
---@class Timer
local M = class("Timer")

require "System.Lang.Collection.Array"
require "System.Lang.Helper.TimerProfiler"

local pairs = pairs
local getCurrentTime = getCurrentTime

function M:ctor()
    self._nextTimerId = 1

    -- self.MIN_INTERVAL = 0.0166666  --1/60帧率设置变了要接收ON_FRAME_RATE_CHANGED事件。这边用不到。

    --每一种定时器的时间粒度不一样，分开保存，对插入和删除操作更好。

    --1. 下一帧触发，只触发一次的，生命周期非常短，用数组保存，可以重用数组。
    self._nextFrameTimers = Array.new(50)

    --2. 每一帧触发，而且不会自己结束，生命周期通常是最长的，也最稳定，用数组保存。其实可以放在Director里去注册。
    self._eachFrameTimers = Array.new(50)

    --3. 一般定时器，生命周期不稳定，插入和删除操作最频繁.
    --3.1 分了2级，_TimeThrehold秒之内会执行的放到工作队列中，不会执行的进入等待队列。每一帧只遍历工作队列中的定时器。
    --3.2 当时间达到等待队列中最早执行时间时，把等待队列中接下来_TimeThrehold秒之内会执行的放到工作队列中。
    --3.3 重复前面2步。可以让遍历的次数大大减少：原来每帧遍历全部的，变成每帧只遍历工作队列。
    --    _TimeThrehold越小，工作队列长度越小，效率越高，但是3.2的执行次数就上升了。
    self._workingTimerQueue = Array.new(50)                 --工作队列.
    self._waitingTimerQueue = Array.new(50)                 --等待队列.
    self._TimeThrehold = 1                 --这么长时间以内可以执行的定时器放到工作队列中，其它的等待. 
    self._earliestTriggerTime = math.huge  --等待队列中最早能执行的时间，不到这个时间就继续等着，到了这个时间就扫描一遍等待队列，把下一批定时器放到工作队列中去.

    --等待添加的定时器
    self._waitingNextFrameTimers = Array.new(10)
    self._waitingEachFrameTimers = Array.new(10)
    self._waitingCommonTimers = Array.new(10)

    --所有定时器会用字典保存起来，为了 unschedule 的时候能快一些
    ---@type TimerNode[]
    self._timerDict = {}  --会增加内存，牺牲空间换时间

    --锁.
    self._isRunning = false
    self._waitingClearAll = false

    --注册update回调.
    self._updateId = false

    --开关
    TimerProfiler.setEnable(__ENBALE_PROFILER__)
end

function M:init()
    if not self._updateId then
        self._updateId = Engine:registerUpdateHandler(function(dt, currentTime, preTime)
            self:update(dt, currentTime, preTime)
        end)
    end
end


function M:_removeWorkingQueueAt(i)
    return self._workingTimerQueue:quickRemove(i)
end
function M:_pushNodeToWorkingQueue(node)
    if not node then
        return
    end

    self._workingTimerQueue:insertLast(node)
end

function M:_removeWaitingQueueAt(i)
    return self._waitingTimerQueue:quickRemove(i)
end
function M:_pushNodeToWaitingQueue(node)
    if not node then
        return
    end

    --等待队列插入一个
    self._waitingTimerQueue:insertLast(node)

    --刷新下次更新的最早时间.
    local nextExecuteTime = node:getNextExecuteTime()
    if nextExecuteTime < self._earliestTriggerTime then
        self._earliestTriggerTime = nextExecuteTime
    end
end

function M:_addWaitingTimers()
    if self._waitingNextFrameTimers:size() > 0 then
        local node
        while self._waitingNextFrameTimers:size() > 0 do
            node = self._waitingNextFrameTimers:removeLast()

            if node:isAborted() then
                self:_releaseTimerNode(node)
            else
                self._nextFrameTimers:insertLast(node)
            end
        end
    end

    if self._waitingEachFrameTimers:size() > 0 then
        local node
        while self._waitingEachFrameTimers:size() > 0 do
            node = self._waitingEachFrameTimers:removeLast()

            if node:isAborted() then
                self:_releaseTimerNode(node)
            else
                self._eachFrameTimers:insertLast(node)
            end
        end
    end

    if self._waitingCommonTimers:size() > 0 then
        local node
        while self._waitingCommonTimers:size() > 0 do
            node = self._waitingCommonTimers:removeLast()

            if node:isAborted() then
                self:_releaseTimerNode(node)
            else
                --在下次调整之前就能执行的，放在工作队列中.
                local nextExecuteTime = node:getNextExecuteTime()
                if nextExecuteTime < self._earliestTriggerTime then
                    self:_pushNodeToWorkingQueue(node)
                else
                    --add to waiting queue and update the earliest time
                    self:_pushNodeToWaitingQueue(node)
                end
            end
        end
    end
end

local frameId = 0
function M:update(dt, currentTime, preTime)
    if __ENBALE_PROFILER__ then
        frameId = frameId + 1
        TimerProfiler.start(frameId)
    end

    --添加等待中的定时器.
    self:_addWaitingTimers()

    self._isRunning = true

    --1. next frame timers
    self:_updateNextFrameTimers(dt, currentTime, preTime)

    --2. each frame timers
    self:_updateEachFrameTimers(dt, currentTime, preTime)

    --3. common timers
    self:_updateCommonTimers(dt, currentTime, preTime)

    self._isRunning = false

    --清除所有.
    if self._waitingClearAll then
        self:unscheduleAll()
    end
end

--下一帧执行的，也是一次性，执行完就回收
function M:_updateNextFrameTimers(dt, currentTime, preTime)
    if self._nextFrameTimers:size() > 0 then
        -- local startTime = os.clock()
        local node
        while self._nextFrameTimers:size() > 0 do
            node = self._nextFrameTimers:removeLast()

            if not node:isAborted() then
                node:trigger()
                node:abort()
            end

            self:_releaseTimerNode(node)
        end
    end
end

--每一帧执行，这边对顺序不敏感，可以倒顺遍历
function M:_updateEachFrameTimers(dt, currentTime, preTime)
    if self._eachFrameTimers:size() > 0 then
        local node
        for i = self._eachFrameTimers:size(), 1, -1 do
            node = self._eachFrameTimers:get(i)

            if not node:isAborted() then
                node:update(dt, currentTime, preTime)
            end

            --直接回收.
            if node:isAborted() then
                self:_releaseTimerNode(node)

                self._eachFrameTimers:quickRemove(i)
            end
        end

    end
end


function M:_updateCommonTimers(dt, curTime, preTime)
    curTime = curTime or self:_getTime()

    --等待列表中也有定时器可以触发了，以及N秒之内会触发的，全选出来，放到工作列表中去
    --同时计算剩下的定时器中最早触发的时间
    if curTime >= self._earliestTriggerTime then
        local node

        --log(">> some timers can be triggered in the _waitingTimerQueue now")
        local newEarliestTriggerTime = math.huge

        for i = self._waitingTimerQueue:size(), 1, -1 do
            node = self._waitingTimerQueue:get(i)

            --如果已经被注销.
            if node:isAborted() then
                self:_releaseTimerNode(node)

                --remove from waiting queue
                self:_removeWaitingQueueAt(i)
            else
                local nextExecuteTime = node:getNextExecuteTime()

                --只要在下一波的_TimeThrehold内执行的都拿出来.
                if nextExecuteTime <= self._earliestTriggerTime + self._TimeThrehold then
                    --remove from waiting queue
                    self:_removeWaitingQueueAt(i)

                    --add to working queue
                    self:_pushNodeToWorkingQueue(node)
                else
                    --just keep it.
                    --还有很久才能执行的，选出最早执行的时间。
                    if nextExecuteTime < newEarliestTriggerTime then
                        newEarliestTriggerTime = nextExecuteTime
                    end
                end
            end
        end

        self._earliestTriggerTime = newEarliestTriggerTime
    end
    --end waiting queue

    --刷新工作中的定时器
    for i = self._workingTimerQueue:size(), 1, -1 do
        local node = self._workingTimerQueue:get(i)

        --如果已经被注销.
        if node:isAborted() then
            self:_releaseTimerNode(node)

            self:_removeWorkingQueueAt(i)
        else
            local nextExecuteTime = node:getNextExecuteTime()

            if curTime >= nextExecuteTime then
                node:trigger()

                --是否可以销毁
                if node:isExhausted() then
                    node:abort()

                    self:_releaseTimerNode(node)

                    self:_removeWorkingQueueAt(i)
                else
                    --interval可能为0，这样nextExecuteTime总是等于当前时间，不过这边没影响，最后都是在工作队列中
                    nextExecuteTime = curTime + node:getInterval()
                    node:setNextExecuteTime(nextExecuteTime)

                    --在下次调整之前就能执行的，放在工作队列中.
                    if nextExecuteTime < self._earliestTriggerTime then
                        --keep it
                    else
                        self:_removeWorkingQueueAt(i)

                        --add to waiting queue and update the earliest time
                        self:_pushNodeToWaitingQueue(node)
                    end
                end
            end
        end
        --end if isAborted()
    end
    --end working queue
end

function M:_getTime()
    return getCurrentTime()
end

function M:_newTimerId()
    local timerId = self._nextTimerId
    self._nextTimerId = self._nextTimerId + 1
    return timerId
end

--object pool
function M:_getTimerNode()
    if TimerNodePool then
        return TimerNodePool:get()
    else
        return TimerNode.new()
    end
end
function M:_releaseTimerNode(timerNode)
    --从查询字典中删除.
    self:_removeTimerNodeFromSearchDict(timerNode:getId())

    if TimerNodePool then
        TimerNodePool:release(timerNode)
    else
        timerNode:reset()
    end
end

--search timer node dict
function M:_addTimerNodeToSearchDict(timerNode)
    local timerId = timerNode:getId()
    if not self._timerDict[timerId] then
        self._timerDict[timerId] = timerNode
        -- print(101, "add a timer node", timerId)
    else
        print(101, "[warning] Duplicated timer node _timerDict? ", timerId)
    end
end

function M:_removeTimerNodeFromSearchDict(timerId)
    if self._timerDict[timerId] then
        self._timerDict[timerId] = nil
        -- print(101, "remove the timer node", timerId)
    end
end

function M:_abortTimerNodeBySearchDict(timerId)
    if self._timerDict[timerId] then
        if not self._timerDict[timerId]:isAborted() then
            self._timerDict[timerId]:abort()
        end
        return true
    else
        -- for k,v in pairs(self._timerDict) do
        --     print(101, k, v:getId())
        -- end
        -- print(101, string.format("[warning] Where is the TimerNode (%d) from? It may be already removed, or never be scheduled.", timerId))
    end
    return false
end

---------------APIs-------------
function M:schedule(func, interval, times)
    if interval == nil or interval < 0 then
        interval = 0
    end
    if times == nil or times < 0 then
        times = 0
    end

    if interval == 0 and times == 1 then
        return self:scheduleNextFrame(func)
    end

    if interval == 0 and times == 0 then
        return self:scheduleEachFrame(func)
    end

    --初始化
    local timerId = self:_newTimerId()

    local node = self:_getTimerNode()
    node:initTimer(interval, times, func, timerId)

    --下一次执行时间.
    local curTime = self:_getTime()
    local nextExecuteTime = curTime + interval

    node:setNextExecuteTime(nextExecuteTime)
    -- log("scheduled a timer", "interval=", interval, "times=", times, "id=", timerId)

    -- 保存到查询字典中.
    self:_addTimerNodeToSearchDict(node)

    --添加
    if self._isRunning then
        self._waitingCommonTimers:insertLast(node)
    else
        --开始全部都放在等待队列中，为了初始化self._earliestTriggerTime
        if self._earliestTriggerTime == math.huge then
            --add to waiting queue and update the earliest time
            self:_pushNodeToWaitingQueue(node)
        else
            --在下次调整之前就能执行的，放在工作队列中.
            if nextExecuteTime < self._earliestTriggerTime then
                self:_pushNodeToWorkingQueue(node)
            else
                self:_pushNodeToWaitingQueue(node)
            end
        end
    end

    return timerId
end

function M:scheduleOnce(interval, func)
    return self:schedule(func, interval, 1)
end

function M:scheduleNextFrame(func)
    local timerId = self:_newTimerId()
    local node = self:_getTimerNode()
    node:initTimer(0, 1, func, timerId)

    -- 保存到查询字典中.
    self:_addTimerNodeToSearchDict(node)

    if self._isRunning then
        self._waitingNextFrameTimers:insertLast(node)
    else
        self._nextFrameTimers:insertLast(node)
    end

    return timerId
end

function M:scheduleEachFrame(func)
    local timerId = self:_newTimerId()
    local node = self:_getTimerNode()
    node:initTimer(0, 0, func, timerId)

    -- 保存到查询字典中.
    self:_addTimerNodeToSearchDict(node)

    if self._isRunning then
        self._waitingEachFrameTimers:insertLast(node)
    else
        self._eachFrameTimers:insertLast(node)
    end

    return timerId
end

--Async Operation，异步操作
function M:unschedule(timerId)
    --根据每种定时器的撤销操作的可能性按大小排序。
    if not timerId then
        return false
    end

    -- 在查询字典中找比较快.
    return self:_abortTimerNodeBySearchDict(timerId)
end

--Async Operation，异步操作
function M:unscheduleAll()
    if self._isRunning then
        self._waitingClearAll = true
        return
    end

    --清空查询字典.
    for k,v in pairs(self._timerDict) do
        v:abort()
        self:_releaseTimerNode(v)
    end
    self._timerDict = {}

    --next frame
    self._nextFrameTimers:clear()

    --each frame
    self._eachFrameTimers:clear()

    --working queue
    self._workingTimerQueue:clear()

    --waiting queue
    self._waitingTimerQueue:clear()


    --waiting next frame
    self._waitingNextFrameTimers:clear()

    --waiting each frame
    self._waitingEachFrameTimers:clear()

    --waiting common
    self._waitingCommonTimers:clear()


    self._waitingClearAll = false
end

------------------------------扩展----------------------------------
-- 延迟定时器
function M:delayInterval(timerId, interval)
    local node = self._timerDict[timerId]
    if node then
        if interval > node:getInterval() then
            self:resetInterval(timerId, interval)
            return true
        end
    end
    return false
end

-- 除非自己知道是短时设置使用，不要随便调这个
function M:resetInterval(timerId, interval)
    if interval < 0 then
        printWarning("[Timer] interval less than 0")
        return false
    end
    local node = self._timerDict[timerId]
    if node then
        node:setInterval(interval)
        node:setNextExecuteTime(self:_getTime() + interval)
        return true
    end
    return false
end

--执行到duration为止,每interval执行一次
--配合缓动类做效果
function M:scheduleDuration(interval,duration,pollFunc,endCall)
    local T = {}
    T.useTime = 0
    T.timer = false
    T.timer = self:schedule(function (...)
        if T.useTime > duration then
            if T.timer then
                Timer:unschedule(T.timer)
                T.timer = false
                if (type(endCall) == "function") then
                    endCall(T.useTime)
                end
                return 
            end
        end
        pollFunc(T.useTime) --耗时
        T.useTime = T.useTime + interval
    end,interval)

    return T.timer
end

rawset(_G, "Timer", false)
Timer = M.new()
