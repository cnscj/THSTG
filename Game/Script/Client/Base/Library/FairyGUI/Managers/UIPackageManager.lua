local FairyGUI = CS.FairyGUI
local M = class("UIPackageManager",false,{
    LoadMode = {
        Resource = 1,
        AssetBundle = 2,
    },
    LoadMethod = {
        Async = 1,
        Sync = 2,
    }
})
function M:ctor()
    self._loadMode = M.LoadMode.Resource  --加载模式,AB或者编辑器模式
    self._loadMethod = M.LoadMode.Sync

    self._packageInfosDict = false
    self._dependenciesDict = false
end

function M.isLoadedPackage(packageName)
    local packageInfo = self:_getPackageInfo(packageName)
    return packageInfo and true or false
end

function M:loadPackage(path,callback)
    local packageName
    if self:isLoadedPackage(packageName) then
        return
    end




end

function M:unloadPackage(path)
    if not self:isLoadedPackage(packageName) then
        return
    end

    --依赖释放

end

function M:addPackage(packageName)

end

function M:removePackage(packageName)
    
end

---
function M:createObject(packageName, componentName)
    local obj = FairyGUI.UIPackage.CreateObject(packageName, componentName)
    return obj
end

function M:isItemExist(path)
    return FairyGUI.UIPackage.GetItemByURL(path)
end
---
function M:_getOrCreatePackageDict()
    self._packageInfosDict = self._packageInfosDict or {}
    return self._packageInfosDict
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

function M:_removePackageInfo(packageName)
    if string.isEmpty(packageName) then 
        return false
    end

    
end

function M:_addPackageInfo(package)
    local packageInfo = {
        refCount = 1,
        path = path,
        package = package,
    }
end

rawset(_G, "UIPackageManager", false)
UIPackageManager = M.new()