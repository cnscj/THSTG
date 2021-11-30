
local M = class("AssetBaseLoader")

--
function M:ctor()
    self.maxLoadingCount = 30   --最大同时加载个数

    self._allHandlerDict = {}
    self._readyHandlers = Queue.new()
    self._loadingHandlers = Set.new()
    self._finishedHandlers = Queue.new()

    --注册更新
    CSharp.MonoManagerIns:AddUpdateListener(function ()
        self:update()
    end)
end

function M:loadAssetSync(path)
    local handler = self:__createHandler(path)
    return self:_onLoadSync(handler)
end

function M:loadAssetAsync(path,onSuccess,onFailed)
    --异步加载把实现延迟到下一帧
    local handler = self:_getOrCreateHandler(path)
    local task = AssetLoaderTask.new()
    task.baseHandler = handler
    task.onSuccess = onSuccess or false
    task.onFailed = onFailed or false
    handler:addCallback(task._onCompleted,task)

    return task
end

function M:update()
    self:dealReady()
    self:dealLoading()
    self:dealFinished()
end

function M:dealReady()
    local curLoadingHandlersNum = self._loadingHandlers:size()
    local canLoadHandlersNum = self.maxLoadingCount - curLoadingHandlersNum
    if self.maxLoadingCount < 0 then canLoadHandlersNum = curLoadingHandlersNum end
    local i = 1
    while i <= canLoadHandlersNum and self._readyHandlers:size() > 0 do
        local handler = self._readyHandlers:dequeue()
        handler:tick()
        self:_onLoadAsync(handler)

        self._loadingHandlers:insert(handler)
        i = i + 1
    end
end

function M:dealLoading()
    for handler in self._loadingHandlers:iter() do 
        if handler:isTimeout() then
            handler:finish()    --标记下,但是是超时失败的返回
            self._loadingHandlers:remove(handler)
            self._finishedHandlers:enqueue(handler)
        elseif handler:isCompleted() then
            self._loadingHandlers:remove(handler)
            self._finishedHandlers:enqueue(handler)
        end
    end
end

function M:dealFinished()
    while self._finishedHandlers:size() > 0 do
        local handler = self._finishedHandlers:dequeue()
        local path = handler.path
        self._allHandlerDict[path] = nil
    end
end

---

function M:__createHandler(path)
    local handler = AssetLoaderHandler.new()
    handler.baseLoader = self
    handler.path = path 
    return handler
end

function M:_getOrCreateHandler(path)
    local handler = self._allHandlerDict[path]
    if not handler then
        handler = self:__createHandler(path)

        self._allHandlerDict[path] = handler
        self._readyHandlers:enqueue(handler)

    end
    return handler
end

---
--[[由子类去实现]]
function M:_onLoadAsync(loaderHandler)

end

function M:_onLoadSync(loaderHandler)

end


rawset(_G, "AssetBaseLoader", M)