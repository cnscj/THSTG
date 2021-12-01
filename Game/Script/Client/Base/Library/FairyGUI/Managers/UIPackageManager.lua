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
    self._loadMode = M.LoadMode.Editor  --加载模式,AB或者编辑器模式

    self._packageInfosDict = false
    self._dependenciesDict = false
    self._itemExistDict = false
end

function M.isLoadedPackage(packageName)
    local packageInfo = self:_getPackageInfo(packageName)
    return packageInfo and true or false
end

function M:loadPackage(path,loadMethod,onSuccess,onFailed)
    if loadMethod == true then 
        loadMethod = M.loadMethod.Async
    elseif loadMethod == false then 
        loadMethod = M.loadMethod.Sync
    end
    
    local packageName = self:getPackageNameByPath(path)
    if self:isLoadedPackage(packageName) then
        return
    end
    
    if loadMethod == M.LoadMethod.Async then
        if self._loadMode == M.LoadMode.Editor then
            self:_onLoadEditorAsync(path,onSuccess,onFailed)
        elseif self._loadMode == M.LoadMode.AssetBundle then
            self:_onLoadAssetBundleAsync(path,onSuccess,onFailed)
        end
    elseif loadMethod == M.LoadMethod.Sync then
        if self._loadMode == M.LoadMode.Editor then
            self:_onLoadEditorSync(path,onSuccess,onFailed)
        elseif self._loadMode == M.LoadMode.AssetBundle then
            self:_onLoadAssetBundleSync(path,onSuccess,onFailed)
        end
    end
end

function M:unloadPackage(path)
    local packageName = self:getPackageNameByPath(path)
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
function M:getPackageNameByPath(path)
    return path
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

function M:_queryDependencies(packageName)

end

--
function M:_onLoadEditorAsync(path,onSuccess,onFailed)

end

function M:_onLoadEditorSync(path,onSuccess,onFailed)
    local package = FairyGUI.UIPackage.AddPackage(path)
end

function M:_onLoadAssetBundleAsync(path,onSuccess,onFailed)

end

function M:_onLoadAssetBundleSync(path,onSuccess,onFailed)

end


function M:_onUnload(path)
    local packageName = self:getPackageNameByPath(path)
    FairyGUI.UIPackage.RemovePackage(packageName)
end
rawset(_G, "UIPackageManager", false)
UIPackageManager = M.new()