local M = class("ObjectPool")

function M:ctor(Type)
    self.type = Type
    self.maxCount = 40

    self._queue = Queue.new()
end

function M:getOrCreate()
    local obj
    if self._queue:size() <= 0 then
        local newObj = self.type.new()
        self:release(newObj)
    end

    obj = self:dequeue()
    return obj
end
--TODO:
function M:release(obj)
    if not obj then
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

end