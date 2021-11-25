
local M = class("AssetBundleLoader",AssetBaseLoader)

local function sqlitePaths(path)
    local bundlePath = false
    local assetPath = false 
    if not string.isEmpty(path) then
        local pathArray = string.split(path,"|")
        bundlePath = pathArray and pathArray[1]
        assetPath = pathArray and pathArray[2]
    end

    return bundlePath,assetPath
end

local function __loadBundleAsync(loaderHandler)
    local path = loaderHandler.path
    local bundlePath,assetPath = sqlitePaths(path)

    local isDone = false
    local resultData = false
    local bundleRequest = CS.UnityEngine.AssetBundle.LoadFromFileAsync(bundlePath)
    while (not bundleRequest.isDone) do
        coroutine.yield() 
    end

    coroutine.yield(bundleRequest)

    if assetPath then
        local ab = bundleRequest.assetBundle
        if ab then
            local assetRequest = ab:LoadAssetAsync(assetPath)

            coroutine.yield(assetRequest)

            isDone = assetRequest.isDone
            resultData = assetRequest.asset
        end
    else
        isDone = bundleRequest.isDone
        resultData = bundleRequest.assetBundle    
    end

    loaderHandler.result = {
        isDone = isDone,
        data = resultData,
    }
    loaderHandler:finish()
end

function M:ctor()
    self._dependenciesPath = false
    self._assetBundleRootPath = false
end

function M:loadManifestPath(mainfestPath)
    local mainfestAssetBundle = CS.UnityEngine.AssetBundle:LoadFromFile(mainfestPath)
    if (mainfestAssetBundle) then

        --取mainfest所在目录为根目录
        self._assetBundleRootPath = PathTool.getDirectoryName(mainfestPath)

        local mainfest = mainfestAssetBundle:LoadAsset("AssetBundleManifest")
        loadManifest(mainfest)

        mainfestAssetBundle:Unload(true)
    end
end

function M:loadManifest(mainfest)
    self._dependenciesPath = {}

    if (mainfest) then
        local allPathsList = mainfest:GetAllAssetBundles()
        for i = 0,allPathsList.Length - 1 do
            local path = allPathsList[i]
            local fullAbPath = PathTool.combine(self._assetBundleRootPath, path)
            local dps = mainfest:GetAllDependencies(path)

            local dewDps = {}
            for j = 0,dps.Length - 1 do
                local dpPath = dps[j]
                local fullDepPath = PathTool.combine(self._assetBundleRootPath, dpPath)
                table.insert( dewDps, fullDepPath)
            end
            
            if (#dewDps > 0) then
                self._dependenciesPath[fullAbPath] = dewDps
            end
        end
    end
end

function M:queryDependencies(bundlePath)
    if self._dependenciesPath then
       return self._dependenciesPath[bundlePath]
    end
end

function M:_onLoadAsync(loaderHandler)
    --依赖加载
    local path = loaderHandler.path
    local bundlePath,assetPath = sqlitePaths(path)
    local dependencies = self:queryDependencies(bundlePath)
    if dependencies then
        for _,dependPath in ipairs(dependencies) do 
            local childHandler = self:_getOrCreateHandler(dependPath)
            loaderHandler:addChild(childHandler)
        end
    end

    local iEnu = coroutine.generator(__loadBundleAsync,loaderHandler)
    coroutine.start(iEnu)
end
rawset(_G, "AssetBundleLoader", M)
return M