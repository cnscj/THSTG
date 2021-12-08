local M = class("UIManager")
local P_View = require("Config.Profile.P_View")
local P_Package = require("Config.Profile.P_Package")
function M:ctor()
    self._packageInfoDict = {}

    self._parentLayerName = {}
    self._parentLayer = {}

    self._openedViews = {}    --正在打开的窗口

    self:setup()
end

function M:setup()
    UIPackageManager.abFolderName = PathConfig.getFGuiEditorPath()

    local packageInfoList = P_Package
    for _,v in ipairs(packageInfoList) do 
        self._packageInfoDict[v.name] = v
    end

    for k, v in pairs(ViewLayer) do
        self._parentLayerName[v] = k
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
        if not self._parentLayer[v] then
            local obj = CSharp.FGUIUtil.CreateLayerObject(v, k .. "Layer")
            self._parentLayer[v] = GComponent.new(obj)
        end
    end

end


function M:initialize()
    self:initPackages()
    self:initViewLayers()
end
-----------------------------------
function M:getViewConfig(viewName)
    return P_View[viewName]
end

function M:getParentLayer(viewDepth)
    if not self._parentLayer[viewDepth]  then
        local layerName = self._parentLayerName[viewDepth]
        if layerName then
            local obj = CSharp.FGUIUtil.CreateLayerObject(viewDepth, layerName .. "Layer")
            self._parentLayer[viewDepth] = GComponent.new(obj)
        end
    end
    return self._parentLayer[viewDepth]
end

--

function M:_newView(viewName,args)
    local viewConfig = self:getViewConfig(viewName)
    if viewConfig then
        local viewCls = require(viewConfig.path)
        if viewCls then
            local view = viewCls.new(nil,args)
            return view
        end
    else
        printError(string.format("No configuration found for View(%s), please check P_View table",viewName))
    end
end


---

function M:convertComponent(obj, userCls, args)
    if not obj then
        return 
    end
    userCls = userCls or GComponent
    return userCls.new(obj, args)
end

--打开非模态窗口
function M:createView(viewName,args)
    local view = self:_newView(viewName,args)
    if not view then return end 

    view:toCreate(function ( ... )
        if not view:getObj() then
            self:closeViewByView(view,true)
            return 
        end
        if view:isClosed() then
            self:closeViewByView(view,true)
            return 
        end
        view:toAdd()
    end)

    self._openedViews[viewName] = self._openedViews[viewName] or {}
    self._openedViews[viewName][view] = view
end

function M:getViews(viewName)
    return self._openedViews[viewName]
end

function M:getView(viewName)
    local dict = self:getViews(viewName)
    return dict and next(dict)
end

--打开模态窗口
function M:openView(viewName,args)
    if self:isViewOpened(viewName) then return end 

    local view = self:_newView(viewName,args)
    if not view then return end 

    local canOpen = view:isCanOpen(args)
    if canOpen ~= true then
        return 
    end

    view:toCreate(function ( ... )
        --如果加载的时候,组件已经接收到关闭命令,则不会在打开
        if not view:getObj() then
            self:closeViewByView(view,true)
            return 
        end
        if view:isClosed() then
            self:closeViewByView(view,true)
            return 
        end
        view:toAdd()
    
    end)

    self._openedViews[viewName] = self._openedViews[viewName] or {}
    self._openedViews[viewName][view] = view
end

function M:closeViewByView(view,isImmediate)
    if not view then return end

    local viewName = view:getViewName()
    local viewDict = self:getViews(viewName)

    view:doClose(isImmediate, true)

    if viewDict and next(viewDict) then 
        viewDict[view] = nil 
        if not next(viewDict) then self._openedViews[viewName] = nil end 
    end

end

function M:closeView(viewName,isImmediate)
    if not self:isViewOpened(viewName) then return end 

    local view = self:getView(viewName)
    self:closeViewByView(view,isImmediate)
end

function M:closeAllViews(viewName,isImmediate)
    if viewName then
        local viewDict = self:getViews(viewName)
        for view in pairs(viewDict) do
            self:closeViewByView(view,isImmediate)
        end
    else    --所有
        for _,dict in pairs(self._openedViews) do
            for _,view in pairs(dict) do
                self:closeViewByView(view,isImmediate)
            end
        end
    end
end

function M:isViewOpened(viewName)
   local view = self:getView(viewName)
   return view and true or false
end

function M:reload()
    --刷新classInstance
end

function M:clear()

end

rawset(_G, "UIManager", false)
UIManager = M.new()