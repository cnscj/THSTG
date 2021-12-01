local M = class("RefObjManager")

function M:ctor()
    self._releaseQueue = Queue.new()

    --注册更新
    MonoManager:addUpdateListener(self.update,self)
end


function M:pushToReleaseList(refObj)
    if not refObj then return end
    self._releaseQueue:enqueue(refObj)
end

function M:update()
    self:dealReleaseObjs()
end

function M:dealReleaseObjs()
    while self._releaseQueue:size() > 0 do
        local refObj = self._releaseQueue:dequeue()
        refObj:release()
    end
end

rawset(_G, "RefObjManager", false)
RefObjManager = M.new()