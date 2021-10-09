local M = class("UIManager")
local P_View = require("Config.Profile.P_View")
local P_Package = require("Config.Profile.P_Package")
function M:ctor()
    self._packageInfoDict = {}
end

function M:setup()
    --
    local packageInfoList = P_Package
    for _,v in ipairs(packageInfoList) do 
        self._packageInfoDict[v.name] = v
    end

    -- TODO:设置加载器
    -- UIPackageManager:
end

-- 加载常驻包
function M:initPackages()
    local packageInfoList = P_Package
    for _,v in ipairs(packageInfoList) do
        while true do 
            if v.loadOpportunity ~= 0 then
                break
            end

            UIPackageManager:addPackage(v.name)

            break
        end
    end
end

function M:getViewConfig( viewName )
    return P_View[viewName]
end

function M:initialize()
    self:setup()

    self:initPackages()
end

function M:openView(viewName)
    
end

function M:closeView(viewName)

end

function M:isViewOpened(viewName)

end

function M:reload()
    --刷新classInstance
end

function M:clear()

end

rawset(_G, "UIManager", false)
UIManager = M.new()