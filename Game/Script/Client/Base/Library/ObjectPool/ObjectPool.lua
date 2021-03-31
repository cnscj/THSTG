local M = class("ObjectPool")

function M:ctor(Type)
    self.maxCount = 60
    self.minCount = 10
    self.idleCleanTime = 60

    self._type = Type
    self._queue = Queue.new()

    self._lastCleanTime = 0
end

function M:getOrCreate()
    local obj
    if self._queue:size() <= 0 then
        if self._type then
            local newObj = self._type.new()
            self:release(newObj)
        end
    end
    if self._queue:size() > 0 then
        obj = self:dequeue()
    end

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
end

function M:clearAll()
    self._queue:clear()
end

--TODO:
function M:update(dt)

end