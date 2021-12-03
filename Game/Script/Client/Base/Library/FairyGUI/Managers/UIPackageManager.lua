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
    self.descSuffix = "_fui.ab"
    self.resSuffix = "_res.ab"


    self._packageInfosDict = false
    self._dependenciesDict = false
    self._itemExistDict = false
end

function M:isLoadedPackage(packageName)
    local packageInfo = self:_getPackageInfo(packageName)
    return packageInfo and true or false
end

function M:loadPackage(path,loadMethod,onSuccess,onFailed)
    loadMethod = loadMethod or false
    if loadMethod == true then 
        loadMethod = M.LoadMethod.Async
    elseif loadMethod == false then 
        loadMethod = M.LoadMethod.Sync
    end

    local packageName = self:getPackageNameByFullPath(path)
    if self:isLoadedPackage(packageName) then
        return
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
    local packageName = self:getPackageNameByFullPath(path)
    if not self:isLoadedPackage(packageName) then
        return
    end

    --依赖释放
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
function M:getPackageNameByFullPath(fullPath)
    return PathTool.getFileNameWithoutExtension(fullPath)
end

function M:getDescPathAndResPathByFullPath(fullPath)
    local packageName = self:getPackageNameByFullPath(fullPath)
    local directorName = PathTool.getDirectoryName(fullPath)
    if string.isEmpty(directorName) then directorName = self.abFolderName end 

    local descFileName = string.format("%s%s", packageName, self.descSuffix)
    local resFileName = string.format("%s%s", packageName, self.resSuffix)

    local descAbFilePath = PathTool.combine(directorName,descFileName)
    local resAbFilePath = PathTool.combine(directorName,resFileName)
    return descAbFilePath,resAbFilePath
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

        self._packageInfosDict[packageName] = packageInfo
    end
    return self._packageInfosDict[packageName]
end

function M:_removePackageInfo(packageName)
    if string.isEmpty(packageName) then 
        return false
    end
end

function M:_queryDependencies(package)
    if not package then return end

    local depList = {}
    local depArray = package.dependencies
    if depArray and depArray.Length > 0 then 
        for i = 0,depArray.Length - 1 do 
            local depPath = depArray[i]
            table.insert( depList, depPath )
        end
    end
    return depList
end

--
function M:_onLoadEditorAsync(path,onSuccess,onFailed)
    Timer:scheduleNextFrame(function ( ... )
        self:_onLoadEditorSync(path,onSuccess,onFailed)
    end)
end

function M:_onLoadEditorSync(path,onSuccess,onFailed)
    --使用AddPackage函数加载desc的bytes文件,需要自定义加载函数
    local package = FairyGUI.UIPackage.AddPackage(path)
    self:_addPackageInfo(package)
end

function M:_onLoadAssetBundleAsync(path,onSuccess,onFailed)
    local callCount = 0
    local descAb = false
    local resAb = false
    local function try2AddPack()
        if callCount >= 2 then
            local package = FairyGUI.UIPackage.AddPackage(descAb,resAb)
            self:_addPackageInfo(package)
        end
    end
    local descPath,resPath = self:getDescPathAndResPathByFullPath(path)
    AssetLoaderManager:loadBundleAssetAsync(descPath,false,function ( result )
        callCount = callCount + 1
        descAb = result.data
        try2AddPack()
    end,function ( ... )
        callCount = callCount + 1
        try2AddPack()
    end)
    AssetLoaderManager:loadBundleAssetAsync(resPath,false,function ( ... )
        callCount = callCount + 1
        resAb = result.data
        try2AddPack()
    end,function ( ... )
        callCount = callCount + 1
        try2AddPack()
    end)
end

function M:_onLoadAssetBundleSync(path,onSuccess,onFailed)
    local descPath,resPath = self:getDescPathAndResPathByFullPath(path)
    local descTask = AssetLoaderManager:loadBundleAssetSync(descPath)
    local resTask = AssetLoaderManager:loadBundleAssetSync(resPath)

    local package = FairyGUI.UIPackage.AddPackage(descTask:getData(),resTask:getData())
    self:_addPackageInfo(package)
end

function M:_onUnload(path)
    local packageName = self:getPackageNameByFullPath(path)
    FairyGUI.UIPackage.RemovePackage(packageName)
end

rawset(_G, "UIPackageManager", false)
UIPackageManager = M.new()