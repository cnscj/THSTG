local M = class("AssetLoader")
local AssetLoaderManagerIns = CSharp.AssetLoaderManagerIns

function M:ctor()

end


function M:loadAssetBundleAsync(abPath,onSuccess,onFailed)
    onSuccess = self:_createSuccessCallback(onSuccess)
    onFailed = self:_createFailedCallback(onFailed)

    local finalPath = abPath
    AssetLoaderManagerIns:LoadAssetAsync(finalPath,onSuccess,onFailed)
end

function M:loadAssetBundleSync(abPath,onSuccess,onFailed)
    onSuccess = self:_createSuccessCallback(onSuccess)
    onFailed = self:_createFailedCallback(onFailed)

    local finalPath = abPath
    AssetLoaderManagerIns:LoadAssetSync(finalPath,onSuccess,onFailed)
end

function M:loadAssetBundleResourceAsync(abPath,resPath,onSuccess,onFailed)
    onSuccess = self:_createSuccessCallback(onSuccess)
    onFailed = self:_createFailedCallback(onFailed)

    local finalPath = string.format("%s|%s",abPath, resPath)
    AssetLoaderManagerIns:LoadAssetAsync(finalPath,onSuccess,onFailed)
end

function M:loadAssetBundleResourceSync(abPath,resPath,onSuccess,onFailed)
    onSuccess = self:_createSuccessCallback(onSuccess)
    onFailed = self:_createFailedCallback(onFailed)

    local finalPath = string.format("%s|%s",abPath, resPath)
    AssetLoaderManagerIns:LoadAssetSync(finalPath,onSuccess,onFailed)
end

function M:_onSuccess(callback, ...)


    if callback then callback(...) end 
end

function M:_onFailed(callback, ...)


    if callback then callback(...) end 
end

--
function M:_createSuccessCallback(callback)
    return function (...) self:_onSuccess(callback, ...) end
end

function M:_createFailedCallback(callback)
    return function (...) self:_onFailed(callback, ...) end
end

rawset(_G, "AssetLoader", false)
AssetLoader = M.new()
