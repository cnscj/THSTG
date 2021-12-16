local FairyGUI = CS.FairyGUI
local M = simpleClass("UIPackageManager")
--加载模式:编辑器模式Bundle模式
M.LoadMode = {
    Editor = 1,
    AssetBundle = 2,
}
--加载方式:同步异步
M.LoadMethod = {
    Async = 1,
    Sync = 2,
}

function M:ctor()
    self.loadMode = M.LoadMode.Editor  --加载模式,AB或者编辑器模式
    self.abFolderName = ""
    self.abSuffix = ".ab"
    self.byteSuffix = ".bytes"
    self.descSuffix = "_fui"
    self.resSuffix = "_res"

    self._packageWrapCache = false
    self._dependenciesDict = false
    self._itemExistDict = false

    self._delayRemoveDict = {}

    MonoManager:addUpdateListener(self.update,self)
end

function M:isLoadedPackage(packageName)
    local packageWrap = self:_getPackageWrap(packageName)
    return packageWrap and true or false
end

function M:loadPackage(path,loadMethod,onSuccess,onFailed)
    local packageName = self:_getPackageNameByFullPath(path)
    local packageWrap = self:_getPackageWrap(packageName)
    if packageWrap then
        if onSuccess then onSuccess(packageWrap) end
        return
    end

    loadMethod = loadMethod or false
    if loadMethod == true then 
        loadMethod = M.LoadMethod.Async
    elseif loadMethod == false then 
        loadMethod = M.LoadMethod.Sync
    end

    if loadMethod == M.LoadMethod.Async then
        if self.loadMode == M.LoadMode.Editor then
            self:_onLoadEditorAsync(path,onSuccess,onFailed)
        elseif self.loadMode == M.LoadMode.AssetBundle then
            self:_onLoadAssetBundleAsync(path,onSuccess,onFailed)
        end
    elseif loadMethod == M.LoadMethod.Sync then
        if self.loadMode == M.LoadMode.Editor then
            self:_onLoadEditorSync(path,onSuccess,onFailed)
        elseif self.loadMode == M.LoadMode.AssetBundle then
            self:_onLoadAssetBundleSync(path,onSuccess,onFailed)
        end
    end
end

function M:unloadPackage(path)
    local packageName = self:_getPackageNameByFullPath(path)
    if not self:isLoadedPackage(packageName) then
        return
    end

    --卸载
    local packageWrap = self:_getPackageWrap(packageName)
    if packageWrap then 
        packageWrap:destroy()
    end
end

function M:retainPackage(packageName)
    local packageWrap = self:_getPackageWrap(packageName)
    if packageWrap then 
        packageWrap:retain()
    end
end

function M:releasePackage(packageName)
    local packageWrap = self:_getPackageWrap(packageName)
    if packageWrap then 
        packageWrap:release()
    end
end

---
function M:isUIExist(url)
    self._itemExistDict = self._itemExistDict or {}
    if not self._itemExistDict[url] then
        self._itemExistDict[url] = FairyGUI.UIPackage.GetItemByURL(url)
    end
    return self._itemExistDict[url]
end

function M:createObject(packageName, componentName)
    if self:isLoadedPackage(packageName) then
        local obj = FairyGUI.UIPackage.CreateObject(packageName, componentName)
        if obj then
            return obj
        else
            error(string.format("The component [%s] is not exported or does not exist.",componentName))
        end
    else
        error(string.format("Package [%s] is not loaded.",packageName))
    end
end

---
function M:getDirectorNameByFullPath(fullPath)
    local directorName = PathTool.getDirectoryName(fullPath)
    if string.isEmpty(directorName) then directorName = self.abFolderName end 
    return directorName
end

function M:_getPackageNameByFullPath(fullPath)
    return PathTool.getFileNameWithoutExtension(fullPath)
end

function M:_getFullPathByPackageName(packageName)
    local directorName = self:getDirectorNameByFullPath(packageName)
    local realPackageName = self:_getPackageNameByFullPath(packageName)
    return PathTool.combine(directorName,realPackageName)
end

function M:_getDescPathAndResPathByFullPath(fullPath)
    local packageName = self:_getPackageNameByFullPath(fullPath)
    local directorName = self:getDirectorNameByFullPath(fullPath)

    local descFileName = string.format("%s%s", packageName, self.descSuffix)
    local resFileName = string.format("%s%s", packageName, self.resSuffix)

    local descAbFilePath = PathTool.combine(directorName,descFileName)
    local resAbFilePath = PathTool.combine(directorName,resFileName)
    return descAbFilePath,resAbFilePath
end

function M:_getDescBundlePathAndResBundlePathByFullPath(fullPath)
    local descPath,resPath = self:_getDescPathAndResPathByFullPath(fullPath)
    return descPath .. self.abSuffix, resPath .. self.abSuffix
end

function M:_getDescBinaryPathByFullPath(fullPath)
    local descPath,resPath = self:_getDescPathAndResPathByFullPath(fullPath)
    return descPath .. self.byteSuffix
end


function M:_getPackageWrap(packageName)
    if not self._packageWrapCache then 
        return false
    end

    if string.isEmpty(packageName) then 
        return false
    end
    
    return self._packageWrapCache[packageName]
end

function M:_addPackageWrap(package)
    if not package then return end 

    local packageName = package.name 
    self._packageWrapCache = self._packageWrapCache or {}
    if not self:_getPackageWrap(packageName) then
        local packageWrap = FairyGUIPackageWrap.new()
        packageWrap.package = package
        packageWrap.onUnwrap = function ( ... )
            local stayTime = packageWrap.stayTime
            if stayTime < 0 then
                return
            elseif stayTime == 0 then
                self:_removePackageWrap(packageName)
            else
                --送入延迟队列
                self._delayRemoveDict[packageName] = packageWrap
            end
        end
        self._packageWrapCache[packageName] = packageWrap
        packageWrap:release(false)
    end
    return self._packageWrapCache[packageName]
end

function M:_removePackageWrap(packageName)
    if string.isEmpty(packageName) then 
        return false
    end

    self._packageWrapCache[packageName] = nil
end

function M:_pollDelayRemoveList()
    if self._delayRemoveDict then
        for packageName,packageWrap in pairs(self._delayRemoveDict) do 
            while true do
                local refCount = packageWrap:refCount()
                if refCount > 0 then 
                    self._delayRemoveDict[packageName] = nil 
                    break 
                else
                    if packageWrap:isStayTimeOut() then
                        self:_removePackageWrap(packageName)
                    end
                end
                break
            end
        end
    end
end

function M:update()
    self:_pollDelayRemoveList()
end

--包依赖查询
function M:queryDependencies(package)
    if not package then return end

    local packageName = package.name
    local depList = self._dependenciesDict and self._dependenciesDict[packageName]
    if not depList then
        depList = {}
        local depArray = CS.THGame.UI.LuaMethodHelper.GetPackageDependencies(package)
        if depArray and depArray.Length > 0 then 
            for i = 0,depArray.Length - 1 do 
                local packageName = depArray[i]
                table.insert( depList, packageName )
            end
        end

        self._dependenciesDict = self._dependenciesDict or {}
        self._dependenciesDict[packageName] = depList
    end

    return depList
end

--
function M:_onLoadCallback(package,path,onSuccess,onFailed)
    if package then
        local packageWrap = self:_addPackageWrap(package)
        if onSuccess then onSuccess(packageWrap) end
    else
        local packageName = self:_getPackageNameByFullPath(path)
        printError(string.format("Package [%s] could not be found",packageName))
        if onFailed then onFailed(path) end
    end
end

function M:_onLoadEditorSync(path,onSuccess,onFailed)
    --使用AddPackage函数加载desc的bytes文件,需要自定义加载函数
    local fullPackageNamePath = self:_getFullPathByPackageName(path)
    local descBinaryPath = self:_getDescBinaryPathByFullPath(path)
    local descTask = AssetLoaderManager:loadBytesAssetSync(descBinaryPath)
    local descBytes = descTask:getData()
    if descBytes then
        local package = CS.THGame.UI.LuaMethodHelper.LoadPackageSyncInPcCustom(descBytes,fullPackageNamePath,function (name, extension, type)
            --Resource type. e.g. 'Texture' 'AudioClip'
            local resBinaryPath = string.format("%s%s", name, extension)
            if type == typeof(CS.UnityEngine.AudioClip) then
                if (extension == ".ogg") then
                    local task = AssetLoaderManager:loadBytesAssetSync(resBinaryPath)
                    local bytes = task:getData()
                    local audioClip = CS.THGame.UI.AudioUtil.ByteToOggAudioClip(bytes)
                    return audioClip
                elseif (extension == ".wav") then
                    local task = AssetLoaderManager:loadBytesAssetSync(resBinaryPath)
                    local bytes = task:getData()
                    local audioClip = CS.THGame.UI.AudioUtil.ByteToWavAudioClip(bytes)
                    return audioClip
                end
            elseif type == typeof(CS.UnityEngine.Texture) then
                local task = AssetLoaderManager:loadBytesAssetSync(resBinaryPath)
                local bytes = task:getData()
                local texture = CS.THGame.UI.TextureUtil.Bytes2Texture2d(bytes)
                return texture
            end
            return
        end)

        self:_onLoadCallback(package,path,onSuccess,onFailed)
    else
        if onFailed then onFailed(path) end
    end
end

function M:_onLoadEditorAsync(path,onSuccess,onFailed)
    --使用AddPackage函数加载desc的bytes文件,需要自定义加载函数
    local fullPackageNamePath = self:_getFullPathByPackageName(path)
    local descBinaryPath = self:_getDescBinaryPathByFullPath(path)
    AssetLoaderManager:loadBytesAssetAsync(descBinaryPath,function ( descResult )
        local descBytes = descResult:getData()
        if descBytes then
            --NOTE:这里需要保证先加载完依赖包才能成功运行
            local package = CS.THGame.UI.LuaMethodHelper.LoadPackageAsyncInPcCustom(descBytes,fullPackageNamePath,function (name, extension, type, call2)
                local function call2CSharp(retObj)
                    if call2 then call2(retObj) end
                end
                --Resource type. e.g. 'Texture' 'AudioClip'
                local resBinaryPath = string.format("%s%s", name, extension)
                if type == typeof(CS.UnityEngine.AudioClip) then
                    if (extension == ".ogg") then
                        AssetLoaderManager:loadBytesAssetAsync(resBinaryPath,function (result)
                            local bytes = result:getData()
                            local audioClip = CS.THGame.UI.AudioUtil.ByteToOggAudioClip(bytes)
                            call2CSharp(audioClip)
                        end,onFailed)
                        
                    elseif (extension == ".wav") then
                        local task = AssetLoaderManager:loadBytesAssetAsync(resBinaryPath,function (result)
                            local bytes = result:getData()
                            local audioClip = CS.THGame.UI.AudioUtil.ByteToWavAudioClip(bytes)
                            call2CSharp(audioClip)
                        end,onFailed)
                    end
                elseif type == typeof(CS.UnityEngine.Texture) then
                    local task = AssetLoaderManager:loadBytesAssetAsync(resBinaryPath,function (result)
                        local bytes = result:getData()
                        local texture = CS.THGame.UI.TextureUtil.Bytes2Texture2d(bytes)
                        call2CSharp(texture)
                    end,onFailed)
                end
            end)

            self:_onLoadCallback(package,path,onSuccess,onFailed)
        else
            if onFailed then onFailed(path) end
        end
    end,onFailed)
end

function M:_onLoadAssetBundleAsync(path,onSuccess,onFailed)
    local callCount = 0
    local descAb = nil
    local resAb = nil
    local function try2AddPack()
        if callCount >= 2 then
            local package = FairyGUI.UIPackage.AddPackage(descAb,resAb)
            self:_onLoadCallback(package,path,onSuccess,onFailed)
        end
    end
    local function addCallCount( ... )
        callCount = callCount + 1
        try2AddPack()
    end

    local descBundlePath,resBundlePath = self:_getDescBundlePathAndResBundlePathByFullPath(path)
    AssetLoaderManager:loadBundleAssetAsync(descBundlePath,false,function ( result )
        descAb = result:getData()
        addCallCount()
    end,addCallCount)
    AssetLoaderManager:loadBundleAssetAsync(resBundlePath,false,function ( result )
        resAb = result:getData()
        addCallCount()
    end,addCallCount)
end

function M:_onLoadAssetBundleSync(path,onSuccess,onFailed)
    local descPath,resPath = self:_getDescBundlePathAndResBundlePathByFullPath(path)

    local descTask = AssetLoaderManager:loadBundleAssetSync(descPath)
    local resTask = AssetLoaderManager:loadBundleAssetSync(resPath)

    local package = FairyGUI.UIPackage.AddPackage(descTask:getData(),resTask:getData())
    self:_onLoadCallback(package,path,onSuccess,onFailed)
end

function M:_onUnload(path)
    local packageName = self:_getPackageNameByFullPath(path)
    FairyGUI.UIPackage.RemovePackage(packageName)
end

---


rawset(_G, "UIPackageManager", false)
UIPackageManager = M.new()