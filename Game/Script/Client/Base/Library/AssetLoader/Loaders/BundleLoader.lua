
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
        local fullBundlePath = PathTool.combine(self._assetBundleRootPath,bundlePath)
        local bundleRequest = CS.UnityEngine.AssetBundle.LoadFromFileAsync(fullBundlePath)
        while (not bundleRequest.isDone) do
            coroutine.yield() 
        end

        coroutine.yield(bundleRequest)

        ab = bundleRequest.assetBundle
        isDone = bundleRequest.isDone
        resultData = ab

        bundleWarp = self:addBundleWrap(bundlePath,ab)
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

    local result = AssetLoaderResult.new()
    result._warp = bundleWarp
    result.isDone = isDone
    result.data = resultData

    loaderHandler.result = result
    loaderHandler:finish()
end

function M:onLoadBundleSync(loaderHandler)
    local path = loaderHandler.path
    local bundlePath,assetPath = sqlitePaths(path)

    local resultData = false
    local bundleWarp = self:getBundleWarp(bundlePath)
    local ab = false
    if not bundleWarp then
        local fullBundlePath = PathTool.combine(self._assetBundleRootPath,bundlePath)
        ab = CS.UnityEngine.AssetBundle.LoadFromFile(fullBundlePath)
        resultData = ab

        bundleWarp = self:addBundleWrap(bundlePath,ab)
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
    self._assetBundleRootPath = false
    self._dependenciesPath = false

    self._bundleWrapCache = false
end

function M:addBundleWrap(bundlePath,assetBundle)
    self._bundleWrapCache = self._bundleWrapCache or {}
    if not self._bundleWrapCache[bundlePath] then
        local bundleWrap = AssetLoaderWrap.new()
        bundleWrap.asset = assetBundle
        bundleWrap.onUnwrap = function ( ... )
           self:_onUnWrap(bundlePath)
        end
        self._bundleWrapCache[bundlePath] = bundleWrap
    end
    return self._bundleWrapCache[bundlePath]
end

function M:removeBundleWrap(bundlePath)
    if self._bundleWrapCache then
        self._bundleWrapCache[bundlePath] = nil
    end
end

function M:getBundleWarp(bundlePath)
    if self._bundleWrapCache then
        return self._bundleWrapCache[bundlePath]
    end
end

function M:loadManifest(manifestPath)
    local manifestAssetBundle = self:loadAssetSync(manifestPath)
    if (manifestAssetBundle) then

        --取manifest所在目录为根目录
        self._assetBundleRootPath = PathTool.getDirectoryName(manifestPath)
        
        local manifest = manifestAssetBundle:LoadAsset("AssetBundleManifest")
        
        self._dependenciesPath = {}
        if (manifest) then
            local allPathsList = manifest:GetAllAssetBundles()
            for i = 0,allPathsList.Length - 1 do
                local path = allPathsList[i]
                local dps = manifest:GetAllDependencies(path)

                local dewDps = {}
                for j = 0,dps.Length - 1 do
                    local dpPath = dps[j]
                    table.insert(dewDps, dpPath)
                end
                
                if (#dewDps > 0) then
                    self._dependenciesPath[path] = dewDps
                end
            end
        end

        manifestAssetBundle:Unload(true)
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

function M:_onUnWrap(bundlePath)
    --如果含有其他依赖,先释放其他依赖在释放自己
    local dependencies = self:queryDependencies(bundlePath)
    if dependencies then
        for _,dependPath in ipairs(dependencies) do 
            local depWarp = self:getBundleWarp(dependPath)
            if depWarp then
                depWarp:release()
            end
        end
    end
    local bundleWrap = self:getBundleWarp(bundlePath)
    local assetBundle = bundleWrap.asset
    if assetBundle then assetBundle:Unload(true) end
    self:removeBundleWrap(bundlePath)
end

rawset(_G, "AssetBundleLoader", M)
return M