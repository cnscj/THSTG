local M = class("AssetManager")
local AssetLoaderManager = CSharp.AssetLoaderManager

function M:ctor()


end

function M:loadAssetBundle(abPath,onSuccess,onFailed)
    onSuccess = self:_createSuccessCallback(onSuccess)
    onFailed = self:_createFailedCallback(onFailed)
    
end

function M:loadAssetBundleResource(abPath,resPath,onSuccess,onFailed)
    onSuccess = self:_createSuccessCallback(onSuccess)
    onFailed = self:_createFailedCallback(onFailed)

end

function M:_createSuccessCallback(callback)
    return function (...)
        self:_onSuccess(callback, ...)
    end
end

function M:_createFailedCallback(callback)
    return function (...)
        self:_onFailed(callback, ...)
    end
end

function M:_onSuccess(callback, ... )


    if callback then callback(...) end 
end

function M:_onFailed(callback, ... )


    if callback then callback(...) end 
end

rawset(_G, "AssetManager", false)
AssetManager = M.new()
