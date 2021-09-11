local M = class("ViewManager")
local P_View = require("Config.Profile.P_View")
local P_Package = require("Config.Profile.P_Package")
function M:ctor()
    self._packageInfoDict = {}
    self._viewsInfoDict = P_View
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

function M:initialize()
    self:setup()

    self:initPackages()
end

function M:open(viewName)
    
end

function M:close(viewName)

end

function M:isOpened(viewName)

end

function M:reload()
    --刷新classInstance
end

function M:clear()

end

rawset(_G, "ViewManager", false)
ViewManager = M.new()