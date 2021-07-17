local M = class("AssetLoader")
local AssetLoaderManagerIns = CSharp.AssetLoaderManagerIns
--[TODO:加载器加载与释放必须成对出现]
function M:ctor()

end

function M:loadAssetBundleAsync(abPath,onSuccess,onFailed)

end

function M:loadAssetBundleSync(abPath,onSuccess,onFailed)
    
end

function M:loadAssetBundleResourceAsync(abPath,resPath,onSuccess,onFailed)

end

function M:loadAssetBundleResourceSync(abPath,resPath,onSuccess,onFailed)

end

function M:loadAssetBundle(isAsync,abPath,onSuccess,onFailed)
    onSuccess = self:_createSuccessCallback(onSuccess)
    onFailed = self:_createFailedCallback(onFailed)

    local finalPath = abPath
    if isAsync then
        AssetLoaderManagerIns:LoadAssetAsync(finalPath,onSuccess,onFailed)
    else
        AssetLoaderManagerIns:LoadAssetSync(finalPath,onSuccess,onFailed)
    end
end
function M:loadAssetBundleResource(isAsync,abPath,resPath,onSuccess,onFailed)
    onSuccess = self:_createSuccessCallback(onSuccess)
    onFailed = self:_createFailedCallback(onFailed)

    local finalPath = string.format("%s|%s",abPath, resPath)
    if isAsync then
        AssetLoaderManagerIns:LoadAssetAsync(finalPath,onSuccess,onFailed)
    else
        AssetLoaderManagerIns:LoadAssetSync(finalPath,onSuccess,onFailed)
    end
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
