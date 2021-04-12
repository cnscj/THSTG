local M = class("AssetManager")
local AssetLoaderManager = CSharp.AssetLoaderManager

function M:ctor()

end

function M:loadAssetBundle(isAsync,objType,abPath,onSuccess,onFailed)
    onSuccess = self:_createSuccessCallback(onSuccess)
    onFailed = self:_createFailedCallback(onFailed)
    
    if isAsync then
        self:loadAssetBundleAsync(objType,abPath,onSuccess,onFailed)
    else
        self:loadAssetBundleSync(objType,abPath,onSuccess,onFailed)
    end
end

function M:loadAssetBundleResource(isAsync,objType,abPath,resPath,onSuccess,onFailed)
    onSuccess = self:_createSuccessCallback(onSuccess)
    onFailed = self:_createFailedCallback(onFailed)

    if isAsync then
        self:loadAssetBundleResourceAsync(objType,abPath,onSuccess,onFailed)
    else
        self:loadAssetBundleResourceSync(objType,abPath,onSuccess,onFailed)
    end
end

function M:loadAssetBundleAsync(objType,abPath,onSuccess,onFailed)
    onSuccess = self:_createSuccessCallback(onSuccess)
    onFailed = self:_createFailedCallback(onFailed)

end

function M:loadAssetBundleSync(objType,abPath,onSuccess,onFailed)

end

function M:loadAssetBundleResourceAsync(objType,abPath,resPath,onSuccess,onFailed)
    onSuccess = self:_createSuccessCallback(onSuccess)
    onFailed = self:_createFailedCallback(onFailed)

end

function M:loadAssetBundleResourceSync(objType,abPath,resPath,onSuccess,onFailed)

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
