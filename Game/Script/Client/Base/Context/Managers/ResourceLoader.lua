local M = class("ResourceLoader")

function M:initialize()
    --TODO:应该获取相对路径
    local abRootPath = PathTool.combine(PathConfig.getResourcePath(),"pc")
    AssetLoaderManager:getOrCreateBundlerLoader():loadManifest(PathTool.combine(abRootPath,"pc"))
end


function M:loadModel(id,onSuccess,onFailed)
    local pathPattern = PathConfig.getModelPatternPath()

    local abPath = PathConfig.normalizePath(pathPattern.abPattern,{id = id})
    local resPath = PathConfig.normalizePath(pathPattern.resPattern,{id = id})
    local fullPath = string.format("%s|%s",abPath,resPath)
    AssetLoaderManager:loadBundleAssetAsync(fullPath,false,onSuccess,onFailed)
end

function M:loadEffect(id,onSuccess,onFailed)
    local pathPattern = PathConfig.getEffectPatternPath()

    local abPath = PathConfig.normalizePath(pathPattern.abPattern,{id = id})
    local resPath = PathConfig.normalizePath(pathPattern.resPattern,{id = id})
    local fullPath = string.format("%s|%s",abPath,resPath)
    AssetLoaderManager:loadBundleAssetAsync(fullPath,false,onSuccess,onFailed)
end

rawset(_G, "ResourceLoader", false)
ResourceLoader = M.new()