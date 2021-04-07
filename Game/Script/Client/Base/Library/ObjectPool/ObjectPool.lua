local M = class("ObjectPool")

function M:ctor(Type)
    self.maxCount = 60
    self.minCount = 10
    self.idleCleanTime = 60 --s

    self._type = Type
    self._queue = Queue.new()

    self._lastCleanTimestampMs = 0
end

function M:getOrCreate()
    local obj
    if self._queue:size() <= 0 then
        if self._type then
            local newObj = self._type.new()
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

    self:_refreshTimestamp()
    return obj
end

function M:release(obj)
    if not obj then
        return
    end
    if not self._type then
        return 
    end

    if obj.__cname ~= self._type.cname then
        return 
    end

    if self.maxCount >= 0 then
        if self._queue:size() >= self.maxCount then
            return
        end
    end

    self._queue:enqueue(obj)

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
    while (self._queue:size() > 0 and self._queue:size() > self.minCount) do 
        self._queue:dequeue()
    end

    self._lastCleanTimestampMs = curTimestamp
end

function M:_refreshTimestamp()
    local curTimestamp = millisecondNow()
    self._lastCleanTimestampMs = curTimestamp
end

rawset(_G, "ObjectPool", M)