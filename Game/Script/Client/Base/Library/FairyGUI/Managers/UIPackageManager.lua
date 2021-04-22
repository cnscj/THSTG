local M = class("UIPackageManager")

function M:ctor()
    
end

function M:loadPackage(packageName, callback)

end

function M.isLoadedPackage(packageName)
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