---@class GComponent:GObject @FGUIComponent
local M = class("GComponent", GObject)

function M:ctor(obj, args)
    self._children = false
    self._controllers = false
    self._scrollPane = false
end

function M:init(obj)
    
end

--清空该类的引用
function M:destroy()
    if self._scrollPane then
        self._scrollPane:destroy()
        self._scrollPane = false
    end

    --先destroy children
    if self._children then
        for k,v in pairs(self._children) do
            if v and v ~= self then
                if type(v) == "table" and type(v.destroy) == "function" then
                    v:destroy()
                else
                    printWarning("GComponent has no destroy function?")
                end
            end
        end
        self._children = false
    end

    if self._controllers then
        for k,v in pairs(self._controllers) do
            if v.destroy then
                v:destroy()
            else
                printWarning("GController has no destroy function?")
            end
        end
        self._controllers = false
    end

    self:super("GObject", "destroy")
end

---@return GController
function M:getController(name)
    if self._controllers and self._controllers[name] then
        return self._controllers[name]
    end
    local ctrlObj = self._obj:GetController(name)
    if ctrlObj then
        local ctrl = GController.new(ctrlObj)
        self._controllers = self._controllers or {}
        self._controllers[name] = ctrl
        return ctrl
    end
    return false
end


function M:getChildObj(name)
    return self._obj:GetChild(name)
end

--NOTE:如果之前查找过name,再次查找name,两次只是类型不同的话,会取上一个的type,应该取最新type
---@return GComponent
function M:getChild(name, type, args)
    if self._children and self._children[name] then
        return self._children[name]
    end
    if not self._obj then
        return false
    end

    local obj = self._obj:GetChild(name)
    if obj == nil then
        return false
    end

    local child = UIManager:convertComponent(obj, type, args)
    self._children = self._children or {}
    self._children[name] = child

    return child
end

function M:getAllStoreChildren()
    return self._typeToChildren
end

function M:getChildrenObj()
    return self._obj:GetChildren()
end

function M:getChildren(type, args)
    local result = {}
    local CObjArray = self:getChildrenObj()
    for i = 0, CObjArray.Length - 1 do
        local obj = CObjArray[i]
        local child = UIManager:convertComponent(obj, type, args)
        table.insert(result, child)
    end
    return result
end

-- 转换class (禁用)
-- function M:transClass(type, args)
--  local cls = ComponentUtil.getCompByType(type)
--  return cls.new(self._obj, args)
-- end
function M:getObj()
    return self._obj
end

function M:addChild(comp)
    if not self._obj then
        printErrorNoTraceback("[GComponent] addChild失败")
        return
    end
    self._obj:AddChild(comp:getObj())
    
    -- bug在addChild后setName不会改变父节点的children
    -- 这个代码不能删除，删了以后会导致第二次getChild会新创建lua的GComponent
    self._children = self._children or {}
    self._children[comp:getName()] = comp
end
function M:addChildAt(comp, idx)
    self._obj:AddChildAt(comp:getObj(), idx)
end

function M:addChildView(viewName)
    -- 只能加 公共包 or 常驻包 or 自己的包
    local viewInfo = UIManager:getViewConfig(viewName)
    local viewClass = require(viewInfo.path)
    local view = viewClass.new({
        parent = self,
    })
    view:toCreate()
end

function M:removeChild(comp, isDisposed)
    if isDisposed == nil then
        isDisposed = false
    end

    self._obj:RemoveChild(comp:getObj(), isDisposed)

    if self._children then
        self._children[comp:getName()] = nil
    end
end
function M:removeChildren()
    self._obj:RemoveChildren()
end

function M:removeFromParent()
    self._obj:RemoveFromParent()
end

function M:setChildIndex(child, index)
    self._obj:SetChildIndex(child, index)
end

function M:getChildIndex(child)
    ---lua层从1开始
    return self._obj:GetChildIndex(child) + 1
end


function M:setText(text)
    if text then
        self._obj.text = text
    else
        self._obj.text = ""
    end
end


-- scrollPane
---@return GScrollPane
function M:getScrollPane()
    if not self._scrollPane then
        self._scrollPane = GScrollPane.new(self._obj.scrollPane)
    end
    return self._scrollPane
end

function M:getTransition(name)
    return self._obj:GetTransition(name) or false
end

-- 接收拖放之后的【放】
function M:onDrop(func)
    self._obj.onDrop:Add(func)
end

function M:clearOnDrop()
    self._obj.onDrop:Clear()
end

function M:removeAllChildren(isDisposed)
    self._obj:RemoveChildren(0, -1, isDisposed)
end

function M:setZIndex(index)
    local oldIndex = self:getZIndex()
    if oldIndex >= index then
        self._obj.parent:SetChildIndexBefore(self._obj, index)
    else
        self._obj.parent:SetChildIndex(self._obj, index)
    end
end

function M:getZIndex(comp)
    return self._obj.parent:GetChildIndex(self._obj)
end


function M:setChildBefore(child)
    local childIndex = self._obj.parent:GetChildIndex(child:getObj())
    self._obj.parent:SetChildIndexBefore(self._obj, childIndex)
end


function M:swapChildren(child1, child2)
    self._obj:SwapChildren(child1:getObj(), child2:getObj())
end


--仅限所有元素"单行竖排"时用
function M:getFixHeight(count)
    local totalHigh = 0
    local objArray = self:getChildrenObj()
    for i = 0, objArray.Length - 1 do
        if count == nil or i < count then
            local obj = objArray[i]
            totalHigh = totalHigh + obj.height
        end
    end
    return totalHigh
end

--仅限所有元素"单行横排"时用
function M:getFixWidth(count)
    local totalWidth = 0
    local objArray = self:getChildrenObj()
    for i = 0, objArray.Length - 1 do
        if count == nil or i < count then
            local obj = objArray[i]
            totalWidth = totalWidth + obj.width
        end
    end
    return totalWidth
end

function M:getUsedChildren()
    return self._children
end

function M:fullScreen(isWithParent)
    local screenWidth, screenHeight = Director:getScreenSize()
	self:setSize(screenWidth, screenHeight)
	self:addRelation(self._obj.parent, FairyGUI.RelationType.Size)
end

function M:fullSize()
    self:setSize(self._obj.parent.width, self._obj.parent.height)
	self:addRelation(self._obj.parent, FairyGUI.RelationType.Size)
end

function M:getProperty(key)
    return UI.getProperty(self._obj,key)
end

rawset(_G, "GComponent", M)