local M = class("AssetLoaderTask")

function M:ctor()
    self.baseHandler = false

    self.onSuccess = false
    self.onFailed = false
    
end

function M:start()


end

function M:stop()
    
end

function M:clear()
    self:_unload()

end

function M:_unload()

end

function M:_onCompleted(result)
    if result.data then
        if self.onSuccess then self.onSuccess(result) end
    else
        if self.onFailed then self.onFailed(result) end
    end
end
--

rawset(_G, "AssetLoaderTask", M)
