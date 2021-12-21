local M = simpleClass("ResourceLoader")

function M:initialize()
    --应该获取相对路径
    local platform = "pc"
    local abRootPath = PathTool.combine(PathConfig.getResourcePath(),platform)
    AssetLoaderManager:getOrCreateBundlerLoader():loadManifest(PathTool.combine(abRootPath,platform))

    UIPackageManager.loadMode = __DEBUG__ and UIPackageManager.LoadMode.Editor or UIPackageManager.LoadMode.AssetBundle
end

function M:loadModel(id,loadMethod,onSuccess,onFailed)
    local pathPattern = PathConfig.getModelPatternPath()

    local abPath = PathConfig.normalizePath(pathPattern.abPattern,{id = id})
    local resPath = PathConfig.normalizePath(pathPattern.resPattern,{id = id})
    local fullPath = string.format("%s|%s",abPath,resPath)
    AssetLoaderManager:loadBundleAssetAsync(fullPath,false,onSuccess,onFailed)
end

function M:loadEffect(id,loadMethod,onSuccess,onFailed)
    local pathPattern = PathConfig.getEffectPatternPath()

    local abPath = PathConfig.normalizePath(pathPattern.abPattern,{id = id})
    local resPath = PathConfig.normalizePath(pathPattern.resPattern,{id = id})
    local fullPath = string.format("%s|%s",abPath,resPath)
    AssetLoaderManager:loadBundleAssetAsync(fullPath,false,onSuccess,onFailed)
end

function M:loadUIPackage(path,loadMethod,onSuccess,onFailed)
    --这里区分加路径
    local newPath = path
    if UIPackageManager.loadMode == UIPackageManager.LoadMode.AssetBundle then
        local pathPattern = PathConfig.getUIPatternPath()
        newPath = PathConfig.normalizePath(pathPattern.abPattern,{id = path})
    elseif UIPackageManager.loadMode == UIPackageManager.LoadMode.Editor then
        local editorPath = PathConfig.getFGuiEditorPath()
        newPath = PathTool.combine(editorPath,path)
    end
    
    return UIPackageManager:loadPackage(newPath,loadMethod,onSuccess,onFailed)
end

rawset(_G, "ResourceLoader", false)
ResourceLoader = M.new()