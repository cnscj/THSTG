---@class TreeView
local M = class("TreeView")

function M:ctor(obj)
	self._obj = FairyGUI.TreeView(obj)
end

function M:getObj()
	return self._obj
end


function M:init(obj)
end


function M:onClickNode(func)
	-- func(context)
	self._obj.onClickNode:Add(func)
end

function M:treeNodeRender(func)
	-- func(node)
	self._obj.treeNodeRender = func
end

function M:treeNodeCreateCell(func)
	self._obj.treeNodeCreateCell = func
end

function M:treeNodeWillExpand(func)
	local function f(node, expand)
		if expand == true then
			SoundUtil.playUISound("ui_showtreelist")
		else
			SoundUtil.playUISound("ui_hidetreelist")
		end
		func(node, expand)
	end
	self._obj.treeNodeWillExpand = f
	
	-- 可以这里加展开收缩音效
end

function M:dispose()
	self._obj:Dispose()
end


function M:addChild(treeNode)
	self._obj.root:AddChild(treeNode:getObj())
end

function M:removeChild(treeNode)
	self._obj.root:RemoveChild(treeNode)
end

function M:getRoot()
	return self._obj.root
end

function M:getList()
	return self._obj.list
end

function M:getListNode(index)
	return self._obj.list:GetChildAt(index)
end

function M:setListItemCondition(func)
    self._obj.list.listItemCondition = func
end

-- 缩进
function M:setIndent(indent)
	self._obj.indent = indent
end

function M:expandAll()
	self._obj:ExpandAll(self._obj.root)
end

function M:collapseAll()
	self._obj:CollapseAll(self._obj.root)
end




-- 选中
function M:onSelectedData(func)
	return func(data, index, comp, node)
end

-- 当前选中的节点
function M:getSelectedData()

end

function M:getSelectedNode()
	return self._obj:GetSelectedNode()
end

-- 设置父节点
function M:setTreeNodeUrl()
end

function M:getRootChildAt(index)
	if index then
		index = index - 1
	end
	if (index >= 0 and index < self._obj.root.numChildren) then
		return self._obj.root:GetChildAt(index)
	end
end

function M:selectedIndex(index, click)
	local treeNode = self:getRootChildAt(index)
	if treeNode then
		self._obj:AddSelection(treeNode, true)
		if click then
		    self._obj.onClickNode:Call(treeNode)
		end
	end
end 



function M:updateNode(node)
	self._obj:UpdateNode(node)
end

function M:updateNodes(nodes)
	self._obj:UpdateNodes(nodes)
end






-- 待优化
function M:setSelectedIndex(...)

	local index1, index2, index3 = ...

	if index1 then
		index1 = index1 - 1
	end
	if index2 then
		index2 = index2 - 1
	end
	if index3 then
		index3 = index3 - 1
	end

	self._obj.list:ClearSelection()

	if index1 and index2 == nil and index3 == nil then
		local treeNode1 = self._obj.root:GetChildAt(index1)
		if treeNode1 then
			local index = self._obj.list:GetChildIndex(treeNode1.cell)
			GList.new(self._obj.list):setSelectedIndex(index+1, true)
		end
		return
	end

	if index1 and index2 and index3 == nil then
		local treeNode1 = self._obj.root:GetChildAt(index1)
		if treeNode1 then
			self._obj:ExpandAll(treeNode1)
			local treeNode2 = treeNode1:GetChildAt(index2)
			if treeNode2 then
				local index = self._obj.list:GetChildIndex(treeNode2.cell)
				GList.new(self._obj.list):setSelectedIndex(index+1, true)
			end
		end
		return
	end

	if index1 and index2 and index3 then
		local treeNode1 = self._obj.root:GetChildAt(index1)
		if treeNode1 then
			self._obj:ExpandAll(treeNode1)
			local treeNode2 = treeNode1:GetChildAt(index2)
			if treeNode2 then
				self._obj:ExpandAll(treeNode2)
				local treeNode3 = treeNode2:GetChildAt(index3)
				if treeNode3 then
					local index = self._obj.list:GetChildIndex(treeNode3.cell)
					GList.new(self._obj.list):setSelectedIndex(index+1, true)
				end
			end
		end
		return
	end
end


rawset(_G, "TreeView", M)
