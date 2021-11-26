local M = class("AssetLoaderTask")

function M:ctor()
    self.baseHandler = false

    self.onSuccess = false
    self.onFailed = false
    
    self._isCompleted = false
end

function M:stop()
    if self.baseHandler then
        self.baseHandler:removeCallback(self._onCompleted,self)
    end
end


function M:unload()
    if not self.baseHandler then return end 
    if not self._isCompleted then return end 

end

function M:clear()
    self:stop()
    self:unload()

    self.onSuccess = false
    self.onFailed = false
    self.baseHandler = false
end

function M:_onCompleted(result)
    self._isCompleted = true
    if result.data then
        if self.onSuccess then self.onSuccess(result) end
    else
        if self.onFailed then self.onFailed(result) end
    end
end
--

rawset(_G, "AssetLoaderTask", M)
