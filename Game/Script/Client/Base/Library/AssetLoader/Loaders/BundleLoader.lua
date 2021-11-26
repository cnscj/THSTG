
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

function M:onLoadBundleAsync(loaderHandler)
    local path = loaderHandler.path
    local bundlePath,assetPath = sqlitePaths(path)
    local isDone = false
    local resultData = false

    local bundleWarp = self:getBundleWarp(bundlePath)
    local ab = false
    if not bundleWarp then
        local bundleRequest = CS.UnityEngine.AssetBundle.LoadFromFileAsync(bundlePath)
        while (not bundleRequest.isDone) do
            coroutine.yield() 
        end

        coroutine.yield(bundleRequest)

        ab = bundleRequest.assetBundle
        isDone = bundleRequest.isDone
        resultData = ab

        self:addBundleWrap(bundlePath,ab)
    else
        ab = bundleWarp.assetBundle
        isDone = true
        resultData = ab
    end

    if assetPath then
        if ab then
            local assetRequest = ab:LoadAssetAsync(assetPath)

            coroutine.yield(assetRequest)

            isDone = assetRequest.isDone
            resultData = assetRequest.asset
        end
    end

    loaderHandler.result = {
        isDone = isDone,
        data = resultData,
    }
    loaderHandler:finish()
end

function M:onLoadBundleSync(loaderHandler)
    local path = loaderHandler.path
    local bundlePath,assetPath = sqlitePaths(path)

    local resultData = false
    local bundleWarp = self:getBundleWarp(bundlePath)
    local ab = false
    if not bundleWarp then
        ab = CS.UnityEngine.AssetBundle.LoadFromFile(bundlePath)
        resultData = ab

        self:addBundleWrap(bundlePath,ab)
    else
        ab = bundleWarp.assetBundle
        resultData = ab
    end

    if assetPath then
        if ab then
            resultData = ab:LoadAsset(assetPath)
        end
    end

    return resultData
end


function M:ctor()
    self._dependenciesPath = false
    self._assetBundleRootPath = false
    
    self._bundleWrapDict = false
end

function M:addBundleWrap(bundlePath,assetBundle)
    self._bundleWrapDict = self._bundleWrapDict or {}
    if not self._bundleWrapDict[bundlePath] then
        self._bundleWrapDict[bundlePath] = {
            assetBundle = assetBundle,
            refCount = 1,
        }
    end
    return self._bundleWrapDict[bundlePath]
end

function M:removeBundleWrap(bundlePath)
    if self._bundleWrapDict then
        self._bundleWrapDict[bundlePath] = nil
    end
end

function M:getBundleWarp(bundlePath)
    if self._bundleWrapDict then
        return self._bundleWrapDict[bundlePath]
    end
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

    local iEnu = coroutine.generator(self.onLoadBundleAsync,self,loaderHandler)
    coroutine.start(iEnu)
end


function M:_onLoadSync(loaderHandler)
    return self:onLoadBundleSync(loaderHandler)
end


rawset(_G, "AssetBundleLoader", M)
return M