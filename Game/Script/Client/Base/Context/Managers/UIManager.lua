local M = class("UIManager")
local P_View = require("Config.Profile.P_View")
local P_Package = require("Config.Profile.P_Package")
function M:ctor()
    self._packageInfoDict = {}
    self._parentLayer = {}
end

function M:setup()
    --
    local packageInfoList = P_Package
    for _,v in ipairs(packageInfoList) do 
        self._packageInfoDict[v.name] = v
    end

end

-- 加载常驻包
function M:initPackages()
    local packageInfoList = P_Package
    for _,v in ipairs(packageInfoList) do
        while true do 
            if v.loadOpportunity ~= 0 then
                break
            end

            UIPackageManager:loadPackage(v.name)

            break
        end
    end
end

--创建窗口层
function M:initViewLayers()
    for k, v in pairs(ViewLayer) do
        local obj = CSharp.FGUIUtil.CreateLayerObject(v, k .. "Layer")
        self._parentLayer[v] = GComponent.new(obj)
    end
end

function M:getViewConfig( viewName )
    return P_View[viewName]
end

function M:initialize()
    self:setup()

    self:initPackages()
    self:initViewLayers()
end
-----------------------------------

function M.geParentLayer(viewDepth)
    return self._parentLayer[viewDepth]
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