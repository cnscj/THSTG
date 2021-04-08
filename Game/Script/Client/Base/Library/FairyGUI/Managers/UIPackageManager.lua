local M = class("UIPackageManager")

function M:ctor()
    
end

function M:loadPackage(package, callback)
   
end

function M:reloadPackage()
 
end

function M.isLoadedPackage(package)
    local info = UIPackageManager.getPackageInfo(package)
    if info and info.isReleased == false then
        return true
    end 
    return false
end


function M:addPackage(packageName)

end

function M:removePackage(packageName)

end

function M:createObject(package, component)
    local obj = FairyGUI.UIPackage.CreateObject(package, component)
    return obj
end

rawset(_G, "UIPackageManager", false)
UIPackageManager = M.new()