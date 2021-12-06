local FairyGUI = CS.FairyGUI
local M = class("UIPackageManager",false,{
    LoadMode = {
        Editor = 1,
        AssetBundle = 2,
    },
    LoadMethod = {
        Async = 1,
        Sync = 2,
    }
})
function M:ctor()    
    self.loadMode = M.LoadMode.Editor  --加载模式,AB或者编辑器模式
    self.abFolderName = ""
    self.abSuffix = ".ab"
    self.byteSuffix = ".bytes"
    self.descSuffix = "_fui"
    self.resSuffix = "_res"

    self._packageInfosDict = false
    self._dependenciesDict = false
    self._itemExistDict = false

    MonoManager:addUpdateListener(self.update,self)
end

function M:isLoadedPackage(packageName)
    local packageInfo = self:_getPackageInfo(packageName)
    return packageInfo and true or false
end

function M:loadPackage(path,loadMethod,onSuccess,onFailed)
    local packageName = self:getPackageNameByFullPath(path)
    if self:isLoadedPackage(packageName) then
        return
    end

    loadMethod = loadMethod or false
    if loadMethod == true then 
        loadMethod = M.LoadMethod.Async
    elseif loadMethod == false then 
        loadMethod = M.LoadMethod.Sync
    end

    local newSuccess = function(package)
        local dependencies = self:_queryDependencies(package)
        local directorName = self:getDirectorNameByFullPath(path)

        for _,depPackageName in ipairs(dependencies) do 
            local depPackagePath = PathTool.combine(directorName, depPackageName)
            self:loadPackage(depPackagePath,loadMethod,false,onFailed)
        end

        if onSuccess then onSuccess(package) end
    end
    
    if loadMethod == M.LoadMethod.Async then
        if self.loadMode == M.LoadMode.Editor then
            self:_onLoadEditorAsync(path,newSuccess,onFailed)
        elseif self.loadMode == M.LoadMode.AssetBundle then
            self:_onLoadAssetBundleAsync(path,newSuccess,onFailed)
        end
    elseif loadMethod == M.LoadMethod.Sync then
        if self.loadMode == M.LoadMode.Editor then
            self:_onLoadEditorSync(path,newSuccess,onFailed)
        elseif self.loadMode == M.LoadMode.AssetBundle then
            self:_onLoadAssetBundleSync(path,newSuccess,onFailed)
        end
    end
end

function M:unloadPackage(path)
    local packageName = self:getPackageNameByFullPath(path)
    if not self:isLoadedPackage(packageName) then
        return
    end

    --卸载
    local packageInfo = self:_getPackageInfo(packageName)
    if packageInfo then 
        packageInfo:_onRelease()
    end
end

function M:retainPackage(packageName)
    local packageInfo = self:_getPackageInfo(packageName)
    if packageInfo then 
        packageInfo:retain()
    end
end

function M:releasePackage(packageName)
    local packageInfo = self:_getPackageInfo(packageName)
    if packageInfo then 
        packageInfo:release()
    end
end

---
function M:createObject(packageName, componentName)
    local obj = FairyGUI.UIPackage.CreateObject(packageName, componentName)
    return obj
end

function M:isItemExist(path)
    self._itemExistDict = self._itemExistDict or {}
    if not self._itemExistDict[path] then
        self._itemExistDict[path] = FairyGUI.UIPackage.GetItemByURL(path)
    end
    return self._itemExistDict[path]
end
---
function M:getDirectorNameByFullPath(fullPath)
    local directorName = PathTool.getDirectoryName(fullPath)
    if string.isEmpty(directorName) then directorName = self.abFolderName end 
    return directorName
end

function M:getPackageNameByFullPath(fullPath)
    return PathTool.getFileNameWithoutExtension(fullPath)
end

function M:getFullPathByPackageName(packageName)
    local directorName = self:getDirectorNameByFullPath(packageName)
    local realPackageName = self:getPackageNameByFullPath(packageName)
    return PathTool.combine(directorName,realPackageName)
end

function M:getDescPathAndResPathByFullPath(fullPath)
    local packageName = self:getPackageNameByFullPath(fullPath)
    local directorName = self:getDirectorNameByFullPath(fullPath)

    local descFileName = string.format("%s%s", packageName, self.descSuffix)
    local resFileName = string.format("%s%s", packageName, self.resSuffix)

    local descAbFilePath = PathTool.combine(directorName,descFileName)
    local resAbFilePath = PathTool.combine(directorName,resFileName)
    return descAbFilePath,resAbFilePath
end

function M:getDescBundlePathAndRedBundlePathByFullPath(fullPath)
    local descPath,resPath = self:getDescPathAndResPathByFullPath(fullPath)
    return descPath .. self.abSuffix, resPath .. self.abSuffix
end

function M:getDescBinaryPathByFullPath(fullPath)
    local descPath,resPath = self:getDescPathAndResPathByFullPath(fullPath)
    return descPath .. self.byteSuffix
end


function M:_getPackageInfo(packageName)
    if not self._packageInfosDict then 
        return false
    end

    if string.isEmpty(packageName) then 
        return false
    end
    
    return self._packageInfosDict[packageName]
end

function M:_addPackageInfo(package)
    if not package then return end 

    local packageName = package.name 
    self._packageInfosDict = self._packageInfosDict or {}
    if not self:_getPackageInfo(packageName) then
        local packageInfo = FairyGUIPackageWrap.new()
        packageInfo.package = package
        packageInfo.onUnwrap = function ( ... )
            self:_removePackageInfo()
        end
        self._packageInfosDict[packageName] = packageInfo
    end
    return self._packageInfosDict[packageName]
end

function M:_removePackageInfo(packageName)
    if string.isEmpty(packageName) then 
        return false
    end
end


function M:update()

end

--包依赖查询
function M:_queryDependencies(package)
    if not package then return end

    local depList = {}
    local depArray = CS.THGame.UI.LuaMethodHelper.GetPackageDependencies(package)
    if depArray and depArray.Length > 0 then 
        for i = 0,depArray.Length - 1 do 
            local packageName = depArray[i]
            table.insert( depList, packageName )
        end
    end
    return depList
end

--
function M:_onLoadCallback(package,path,onSuccess,onFailed)
    if package then
        self:_addPackageInfo(package)
        if onSuccess then onSuccess(package) end
    else
        local packageName = self:getPackageNameByFullPath(path)
        printError(string.format("Package %s could not be found",packageName))
        if onFailed then onFailed(path) end
    end
end

function M:_onLoadEditorAsync(path,onSuccess,onFailed)
    Timer:scheduleNextFrame(function ( ... )
        self:_onLoadEditorSync(path,onSuccess,onFailed)
    end)
end

function M:_onLoadEditorSync(path,onSuccess,onFailed)
    --使用AddPackage函数加载desc的bytes文件,需要自定义加载函数
    local descBinaryPath = self:getDescBinaryPathByFullPath(path)
    local descTask = AssetLoaderManager:loadBytesAssetSync(descBinaryPath)
    local descBytes = descTask:getData()
    local package = CS.THGame.UI.LuaMethodHelper.LoadPackageInPcCustom(descBytes,function (name, extension, type)
        --Resource type. e.g. 'Texture' 'AudioClip'
        local directorName = self:getDirectorNameByFullPath(path)
        local packageName = self:getPackageNameByFullPath(path)
        local resBinaryPath = PathTool.combine(directorName,string.format("%s_%s%s",packageName,name, extension))

        if type == typeof(CS.UnityEngine.AudioClip) then
            if (extension == ".ogg") then

            elseif (extension == ".wav") then
                local task = AssetLoaderManager:loadBytesAssetSync(resBinaryPath)
                local bytes = task:getData()
                local audioClip = CS.THGame.UI.WavUtil.ToAudioClip(bytes)
                audioClip.name = PathTool.getFileNameWithoutExtension(name)
                return audioClip
            end
        else
            local task = AssetLoaderManager:loadBytesAssetSync(resBinaryPath)
            local bytes = task:getData()
            if type == typeof(CS.UnityEngine.Texture) then
                local texture = CS.THGame.UI.TextureUtil.Bytes2Texture2d(bytes)
                texture.name = PathTool.getFileNameWithoutExtension(name)
                return texture
            end
        end
        
        return
    end)

    self:_onLoadCallback(package,path,onSuccess,onFailed)
end

function M:_onLoadAssetBundleAsync(path,onSuccess,onFailed)
    local callCount = 0
    local descAb = false
    local resAb = false
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

    local descBundlePath,resBundlePath = self:getDescBundlePathAndRedBundlePathByFullPath(path)
    AssetLoaderManager:loadBundleAssetAsync(descBundlePath,false,function ( result )
        descAb = result.data
        addCallCount()
    end,addCallCount)
    AssetLoaderManager:loadBundleAssetAsync(resBundlePath,false,function ( result )
        resAb = result.data
        addCallCount()
    end,addCallCount)
end

function M:_onLoadAssetBundleSync(path,onSuccess,onFailed)
    local descPath,resPath = self:getDescPathAndResPathByFullPath(path)
    local descTask = AssetLoaderManager:loadBundleAssetSync(descPath)
    local resTask = AssetLoaderManager:loadBundleAssetSync(resPath)

    local package = FairyGUI.UIPackage.AddPackage(descTask:getData(),resTask:getData())
    self:_onLoadCallback(package,path,onSuccess,onFailed)
end

function M:_onUnload(path)
    local packageName = self:getPackageNameByFullPath(path)
    FairyGUI.UIPackage.RemovePackage(packageName)
end
---


rawset(_G, "UIPackageManager", false)
UIPackageManager = M.new()