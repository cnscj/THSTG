---@class TreeNode
local M = class("TreeNode")

function M:ctor(obj)
	if type(obj) == "boolean" then
		self._obj = FairyGUI.TreeNode(obj)
	else
		self._obj = obj
	end
	

	-- self.data = false
	-- self.tree = false
	-- self.cell = false
	-- self.level = false
	-- self.expanded = false
	-- self.isFolder = false
	-- self.text = false
	-- self.tree = false
	-- self.numChildren = false
end

-- function M:init()
-- end

function M:getObj()
	return self._obj
end

-- 数据
function M:setData(data)
    self._obj.data = data
end
function M:getData(data)
    return self._obj.data
end

function M:addChild(treeNode)
	self._obj:AddChild(treeNode:getObj())
end

-- 层级
function M:getLevel()
	return self._obj.level
end

function M:getTree()
	return self._obj.tree
end

function M:getCell()
	return self._obj.cell
end

function M:setExpanded(expanded)
	self._obj.expanded = expanded
end

-- 是否折叠
function M:isExpanded()
	return self._obj.expanded
end

-- 是否是文件夹节点
function M:isFolder()
	return self._obj.isFolder
end

-- 重新绘制一遍
function M:refreshNodeRender()
	self._obj.tree:UpdateNode(self._obj)
end

-- function M:addChild(treeNode)
-- 	self._obj:AddChild(treeNode:getObj())
-- end

-- function M:addChild(treeNode)
-- 	self._obj:AddChild(treeNode:getObj())
-- end

-- function M:addChild(treeNode)
-- 	self._obj:AddChild(treeNode:getObj())
-- end

rawset(_G, "TreeNode", M)
