--定时器实体
---@class TimerNode
local M = simpleClass("TimerNode")

function M:ctor()
    --所有字段reset()都包含
    self:reset()
end

function M:initTimer(interval, repeatTimes, callback, id)
    self._interval = interval
    self._repeatTimes = repeatTimes
    self._callback = callback
    self._id = id

    self._runForever = repeatTimes <= 0
    self._elapsed = 0

    self._nextExecuteTime = math.huge

    self._aborted = false
end

function M:reset()
    self._interval = false
    self._repeatTimes = false
    self._callback = false
    self._id = false

    self._runForever = false
    self._elapsed = false
    self._nextExecuteTime = false
    self._aborted = false
end

function M:getId()
    return self._id
end

function M:setInterval(interval)
    self._interval = interval
end

function M:getInterval()
    return self._interval
end

function M:setNextExecuteTime(t)
    self._nextExecuteTime = t
end
function M:getNextExecuteTime()
    return self._nextExecuteTime
end

function M:isAborted()
    return self._aborted
end

function M:abort()
    self._aborted = true
end

--only used for Timer，可以考虑拿出去，不放在类里面.
function M:update(dt)
    self._elapsed = self._elapsed + dt

    --self._interval = 0时，设置为self._elapsed，即每一帧执行一次.
    local intervalTmp = self._interval > 0 and self._interval or self._elapsed

    --trigger one time in one frame
    -- if not self._aborted and self._elapsed >= intervalTmp then
    --     self:trigger()

    --     self._elapsed = self._elapsed - intervalTmp

    --     if self:isExhausted() then
    --         self:abort()
    --     end

    --     return
    -- end

    --trigger multi times in one frame
    while not self._aborted and self._elapsed >= intervalTmp do
        self:trigger()

        self._elapsed = self._elapsed - intervalTmp

        if self:isExhausted() then
            self:abort()
            return
        end

        if self._elapsed <= 0 then
            break
        end
    end
end

--是否可以结束.
function M:isExhausted()
    return (not self._runForever and self._repeatTimes <= 0)
end

local millisecondNow = millisecondNow
local time0, time1
-- local logError = CS.GYGame.Logger.LogError or print
local logWarn = CS.GYGame.Logger.LogWarning or print

function M:trigger()
    if __DEBUG__ or __ENBALE_PROFILER__ then
        time0 = millisecondNow()
    end

    -- 先减少计数，再回调，这样即使报错了，也不会无限触发
    if self._repeatTimes >= 0 then
        self._repeatTimes = self._repeatTimes - 1
    end

    self._callback()

    if __ENBALE_PROFILER__ then
        time1 = millisecondNow()  --ms

        local info = debug.getinfo(self._callback, "Sln")
        local msg = string.format("%s L%d %s", info.source, info.linedefined, info.name and "in " .. info.name or "")
        local time = time1 - time0
        TimerProfiler.record(msg, time)

        if time > 400 then
            logWarn(string.format("这个定时器回调耗时很长(%s ms)，不算错误，但是值得关注:\n%s", tostring(time), msg))
        end
    elseif __DEBUG__ then
        time1 = millisecondNow()  --ms
        local time = time1 - time0

        if time > 400 then
            local info = debug.getinfo(self._callback, "Sln")
            local msg = string.format("%s L%d %s", info.source, info.linedefined, info.name and "in " .. info.name or "")

            logWarn(string.format("这个定时器回调耗时很长(%s ms)，不算错误，但是值得关注:\n%s", tostring(time), msg))
        end
    end
end

rawset(_G, "TimerNode", M)
