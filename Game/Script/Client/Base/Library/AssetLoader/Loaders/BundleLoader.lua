
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
        local fullBundlePath = self:_getBundleFullPath(bundlePath)
        local bundleRequestInfo = self:_getOrCreateBundleRequest(fullBundlePath)
        if bundleRequestInfo then
            local bundleRequest = bundleRequestInfo.bundleRequest
            local requestRefCount = bundleRequestInfo.refCount
            --NOTE:添加下子AB的依赖,写这里感觉不太对
            if requestRefCount == 1 then
                loaderHandler:addCallback(self._onBundleDependencies,self)
            end
            while (not bundleRequest.isDone) do
                coroutine.yield() 
            end

            coroutine.yield(bundleRequest)

            ab = bundleRequest.assetBundle
            isDone = bundleRequest.isDone
            resultData = ab

            bundleWarp = self:addBundleWrap(bundlePath,ab)
            self:_releaseCreateBundleRequest(fullBundlePath)
        end
    else
        ab = bundleWarp.assetBundle
        isDone = true
        resultData = ab
    end

    --在同一帧下,如果A加载包AB,B加载包AB里的C,如果在它们的回调里释放AB,有可能会使得B没跑完加载AB就被释放了导致报错
    --因此杜绝在回调里释放AB
    if assetPath then
        if ab then
            bundleWarp:retain() --先持有下,免得被释放掉了
            local assetRequest = ab:LoadAssetAsync(assetPath)

            coroutine.yield(assetRequest)
            bundleWarp:release(false)    --如果这里超时了可能会造成释放不掉

            isDone = assetRequest.isDone
            resultData = assetRequest.asset
        end
    end

    local loaderResult = AssetLoaderResult.new()
    loaderResult._warp = bundleWarp
    loaderResult.isDone = isDone
    loaderResult.data = resultData

    loaderHandler.result = loaderResult
    loaderHandler:finish()
end

function M:onLoadBundleSync(loaderHandler)
    local path = loaderHandler.path
    local bundlePath,assetPath = sqlitePaths(path)

    local resultData = false
    local isDone = false
    local bundleWarp = self:getBundleWarp(bundlePath)
    local ab = false
    if not bundleWarp then
        local fullBundlePath = self:_getBundleFullPath(bundlePath)
        ab = self:_getOrLoadBundle(fullBundlePath)

        if ab then
            resultData = ab
            isDone = true

            bundleWarp = self:addBundleWrap(bundlePath,ab)

            --添加下子类引用的AB
            self:_onBundleDependencies(loaderHandler)
        end
    else
        ab = bundleWarp.assetBundle
        resultData = ab
        isDone = true
    end

    if assetPath then
        if ab then
            resultData = ab:LoadAsset(assetPath)
        end
    end

    local loaderResult = AssetLoaderResult.new()
    loaderResult._warp = bundleWarp
    loaderResult.isDone = isDone
    loaderResult.data = resultData

    loaderHandler.result = loaderResult
    loaderHandler:finish()

    return loaderResult
end

function M:ctor()
    self._assetBundleRootPath = false
    self._dependenciesPaths = false
    self._bundleRequestCache = false
    self._bundleWrapCache = false
end

function M:addBundleWrap(bundlePath,assetBundle)
    if string.isEmpty(bundlePath) then return end
    if not assetBundle then return end

    self._bundleWrapCache = self._bundleWrapCache or {}
    if not self._bundleWrapCache[bundlePath] then
        local bundleWrap = AssetLoaderWrap.new()
        bundleWrap.asset = assetBundle
        bundleWrap.onUnwrap = function ( ... )
           self:_onUnWrap(bundlePath)
        end
        bundleWrap:release(false) --弱引用释放
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
    local loaderTask = self:loadAssetSync(manifestPath)
    local manifestAssetBundle = loaderTask:getData()
    if (manifestAssetBundle) then

        --取manifest所在目录为根目录
        self._assetBundleRootPath = PathTool.getDirectoryName(manifestPath)
        
        local manifest = manifestAssetBundle:LoadAsset("AssetBundleManifest")
        
        self._dependenciesPaths = {}
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
                    self._dependenciesPaths[path] = dewDps
                end
            end
        end

        loaderTask:release()
    end
end

function M:queryDependencies(bundlePath)
    if self._dependenciesPaths then
       return self._dependenciesPaths[bundlePath]
    end
end

function M:_getBundleFullPath(bundlePath)
    if not string.isEmpty(self._assetBundleRootPath) then
        if string.find(bundlePath,self._assetBundleRootPath) then
            return bundlePath
        end
    end
    return PathTool.combine(self._assetBundleRootPath,bundlePath)
end

function M:_getOrLoadBundle(bundlePath)
    --Note:异常处理
    local bundle = CS.UnityEngine.AssetBundle.LoadFromFile(bundlePath)
    return bundle
end

function M:_getOrCreateBundleRequest(bundlePath)
    self._bundleRequestCache = self._bundleRequestCache or {}
    local bundleRequestInfo = self._bundleRequestCache[bundlePath]
    if not bundleRequestInfo then
        --Note:异常处理
        local bundleRequest = CS.UnityEngine.AssetBundle.LoadFromFileAsync(bundlePath)
        if not bundleRequest then return end 
        self._bundleRequestCache[bundlePath] = {bundleRequest = bundleRequest,refCount = 1}
    else
        bundleRequestInfo.refCount = bundleRequestInfo.refCount + 1
    end
    return self._bundleRequestCache[bundlePath]
end

function M:_releaseCreateBundleRequest(bundlePath)
    if self._bundleRequestCache then
        local bundleRequestInfo = self._bundleRequestCache[bundlePath]
        if bundleRequestInfo then
            bundleRequestInfo.refCount = bundleRequestInfo.refCount - 1
            if bundleRequestInfo.refCount <= 0 then
                self._bundleRequestCache[bundlePath] = nil
            end
        end
    end
end

function M:_onLoadAsync(loaderHandler)
    local path = loaderHandler.path
    local bundlePath,assetPath = sqlitePaths(path)
    local dependencies = self:queryDependencies(bundlePath)    --依赖加载
    if dependencies then
        for _,dependPath in ipairs(dependencies) do 
            local childHandler = self:_getOrCreateAsyncHandler(dependPath)
            loaderHandler:addChild(childHandler)
            self:_loadHandlerAsync(childHandler)
        end
    end

    local iEnu = coroutine.generator(self.onLoadBundleAsync,self,loaderHandler)
    coroutine.start(iEnu)
end


function M:_onLoadSync(loaderHandler)
    local path = loaderHandler.path
    local bundlePath,assetPath = sqlitePaths(path)
    local dependencies = self:queryDependencies(bundlePath)
    if dependencies then
        for _,dependPath in ipairs(dependencies) do 
            local childHandler = self:_createHandler(dependPath)    --不能与异步的Handler混用,否则就不能同步
            loaderHandler:addChild(childHandler)
            self:_loadHandlerSync(childHandler)
        end
    end

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
    if assetBundle then assetBundle:Unload(false) end   --如果不严格使用引用计数错误释放会很麻烦,这里就不进行完全释放了
    self:removeBundleWrap(bundlePath)
end

--增持依赖引用
function M:_onBundleDependencies(loaderHandler)
    local path = loaderHandler.path
    local bundlePath,assetPath = sqlitePaths(path)
    local dependencies = self:queryDependencies(bundlePath)
    if dependencies then
        for _,dependPath in ipairs(dependencies) do 
            local childBundleWarp = self:getBundleWarp(dependPath)
            if childBundleWarp then childBundleWarp:retain() end
        end
    end
end
rawset(_G, "AssetBundleLoader", M)
return M