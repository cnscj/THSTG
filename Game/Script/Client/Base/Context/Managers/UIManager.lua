local M = class("UIManager")
local P_View = require("Config.Profile.P_View")
local P_Package = require("Config.Profile.P_Package")
function M:ctor()
    self._packageInfoDict = {}
    self._packageLoadOpportunityDict = {}

    self._parentLayerName = {}
    self._parentLayer = {}

    self._openedViews = {}    --正在打开的窗口

    self:setup()
end

function M:setup()
    UIPackageManager.loadMode = __DEBUG__ and UIPackageManager.LoadMode.Editor or UIPackageManager.LoadMode.AssetBundle

    local packageInfoList = P_Package
    for _,v in ipairs(packageInfoList) do 
        self._packageInfoDict[v.name] = v

        if v.loadOpportunity then
            self._packageLoadOpportunityDict[v.loadOpportunity] = self._packageLoadOpportunityDict[v.loadOpportunity] or {}
            table.insert(self._packageLoadOpportunityDict[v.loadOpportunity],v)
        end

    end

    for k, v in pairs(ViewLayer) do
        self._parentLayerName[v] = k
    end
end

-- 加载常驻包
function M:initPackages()
   self:loadPackagesByLoadOpportunity(0)    --进入游戏加载
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

function M:initUICamera( ... )
    --摄像机参数设置
    -- CSharp.StageCamera.main.clearFlags = CS.UnityEngine.CameraClearFlags.SolidColor
    -- CSharp.StageCamera.main.backgroundColor.a = 0
end


function M:initialize()
    self:initPackages()
    self:initViewLayers()
    self:initUICamera()
end
-----------------------------------
function M:getPackageConfig(packageName)
    return self._packageInfoDict[packageName]
end

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

function M:_newView(viewName, args)
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
function M:loadPackagesByLoadOpportunity(loadOpportunity)
    local packageList = self._packageLoadOpportunityDict[loadOpportunity]
    if packageList then
        for _,v in ipairs(packageList) do 
            self:loadPackage(v.name)
        end
    end
end

function M:loadPackage(path,loadMethod,onSuccess,onFailed)
    local needCallCount = 1
    local callCount = 0
    local successOrLoadChild = function(packageWrap)    --依赖加载
        local dependencies = UIPackageManager:queryDependencies(packageWrap.package)
        local directorName = UIPackageManager:getDirectorNameByFullPath(path)
        local tryCallback = function ( ... )
            callCount = callCount + 1
            if callCount >= needCallCount then 
                if onSuccess then onSuccess(packageWrap) end
            end
        end

        if dependencies then
            needCallCount = needCallCount + #dependencies
            for _,depPackageName in ipairs(dependencies) do 
                local depPackagePath = PathTool.combine(directorName, depPackageName)
                ResourceLoader:loadUIPackage(depPackagePath,loadMethod,function (childPackageWrap)
                    childPackageWrap:retain() --这里加载完成需要添加下依赖,只加一次即可
                    tryCallback()
                end,function ( ... )
                    tryCallback()
                end)

                --非常驻依赖加载判断
                local packageConfig = self:getPackageConfig(depPackageName)
                if not packageConfig or (packageConfig and not packageConfig.isResident) then
                    printWarning(string.format("A non-resident package is loaded:%s",depPackageName))
                end

            end
        end
        tryCallback()
    end

    return ResourceLoader:loadUIPackage(path,loadMethod,successOrLoadChild,onFailed)
end

function M:createObject(packageName, componentName)
    return UIPackageManager:createObject(packageName, componentName)
end

function M:convertComponent(obj, compType, args)
    if not obj then
        return 
    end
    
    local cls = false
    if type(compType) == "string" then
        cls = CompType[compType]
    elseif type(compType) == "table" then
        cls = compType
    end

    if not cls then
        error(string.format("CompType error type:%s not exist.", compType))
    end
    cls = cls or GComponent
    return cls.new(obj, args)
end

function M:createComponent(packageName, componentName, userCls, args)
    local obj = self:createObject(packageName, componentName)
    if obj then
        return self:convertComponent(obj,userCls,args)
    end
end

--打开新建窗口
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

--打开窗口,只允许一个
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