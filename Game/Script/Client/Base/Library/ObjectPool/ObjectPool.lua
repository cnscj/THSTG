local M = class("ObjectPool")

function M:ctor(cls)
    self.maxCount = 60
    self.minCount = 10
    self.idleCleanTime = 60 --s

    self.getCallback = false
    self.releaseCallback = false
    self.destroyCallback = false

    self._cls = cls
    self._queue = Queue.new()

    self._lastCleanTimestampMs = 0
end

function M:getOrCreate()
    local obj
    if self._queue:size() <= 0 then
        if self._cls then
            local newObj = self._cls.new()
            self:release(newObj)
        end
    end
    obj = self:get()
    
    return obj
end

function M:get()
    local obj
    if self._queue:size() > 0 then
        obj = self._queue:dequeue()
    end
    if self.getCallback then
        self.getCallback(obj)
    end
    self:_refreshTimestamp()
    return obj
end

function M:release(obj)
    if not obj then
        return
    end
    if not self._cls then
        return 
    end

    if obj.__cname ~= self._cls.cname then
        return 
    end

    if self.maxCount >= 0 then
        if self._queue:size() >= self.maxCount then
            return
        end
    end

    self._queue:enqueue(obj)
    if self.releaseCallback then
        self.releaseCallback(obj)
    end
    self:_refreshTimestamp()
end

function M:clearAll()
    self._queue:clear()

    self:_refreshTimestamp()
end

function M:update(dt)
    if self.idleCleanTime <= 0 then 
        return 
    end

    local curTimestamp = millisecondNow()
    if self._lastCleanTimestampMs + self.idleCleanTime * 1000 < curTimestamp then
        return 
    end

    --清空到最小
    --一次性清空会出问题,建议分几帧清
    local startClearTimeMS = millisecondNow()
    while (self._queue:size() > 0 and self._queue:size() > self.minCount) do 
        --取一个硬件时间,如果硬件时间过长则跳出
        local curClearTimeMS = millisecondNow()
        if curClearTimeMS - startClearTimeMS >= 100 then    --100ms才没有明显卡顿
            break
        end

        local obj = self._queue:dequeue()
        if self.destroyCallback then 
            self.destroyCallback(obj)
        end
    end

    self._lastCleanTimestampMs = curTimestamp
end

function M:_refreshTimestamp()
    local curTimestamp = millisecondNow()
    self._lastCleanTimestampMs = curTimestamp
end

rawset(_G, "ObjectPool", M)