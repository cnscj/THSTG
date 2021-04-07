
local M = class("GFriendMenu", GComponent)

function M:ctor()
	self.btn = false
	self.list = false

	self._height = 0
end

function M:init()
	self.btn = self:getChild("btn", "Button")
	self.list = self:getChild("list", "List")
end

function M:setText(text)
	self.btn:setText(text)
end

function M:setList(array)
	self.list:setDataProvider(array)
end

function M:updateList()
	self.list:refreshVirtualList()
end


function M:getListData()
	return self.list:getDataProvider()
end

function M:setListHeight(height)
	self._height = height
end

function M:expand()
	self.btn:setSelected(true)
	self.list:setHeight(self._height)
end

function M:shrink()
	self.btn:setSelected(false)
	self.list:setHeight(0)
end

function M:setSelected(selected)
	self.btn:setSelected(selected)
end

rawset(_G, "GFriendMenu", M)
