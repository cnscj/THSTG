local M = class("AssetLoaderTask")

function M:ctor()
    self.baseHandler = false

    self.onSuccess = false
    self.onFailed = false
    
end

function M:getResult()
    return self.baseHandler.result
end

function M:getData()
    local loaderResult = self:getResult()
    return loaderResult and loaderResult.data
end

function M:stop()
    if not self.baseHandler then return end 
    if self.baseHandler.result then return end 

    self.baseHandler:removeCallback(self._onCompleted,self)
end

function M:retain()
    if not self.baseHandler then return end 
    if not self.baseHandler.result then return end 

    self.baseHandler.result:retain()
end

function M:release()
    if not self.baseHandler then return end 
    if not self.baseHandler.result then return end 

    self.baseHandler.result:release()
end

function M:clear()
    self:stop()
    self:release()

    self.onSuccess = false
    self.onFailed = false
    self.baseHandler = false

end

function M:_onCompleted(handler)
    local result = handler.result
    if result.data then
        self:retain()
        if self.onSuccess then 
            self.onSuccess(result) 
        end
    else
        if self.onFailed then 
            self.onFailed(result) 
        end
    end
end
--

rawset(_G, "AssetLoaderTask", M)
