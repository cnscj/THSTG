---@class GComboBox:GComponent
local M = class("GComboBox", GComponent)

function M:ctor(obj)
	self._listDataProvider = false
	self._dataProvider = false
	self.buttonCtrl = false

	-- 展开的音效
	self._obj.showSound = FairyGUI.UIPackage.GetItemAsset("UISound", "ui_showdroplist")
end

function M:init(obj)
	--控制箭头的移动
	self.buttonCtrl  = self:getController("button")
	if self.buttonCtrl then
		self:onClosed(function ()
			if self._obj.dropdown.isDisposed then return end
			self.buttonCtrl:setSelectedIndex(0)
		end)
	end
end
function M:destroy()
    if self.buttonCtrl then
        self.buttonCtrl:destroy()
        self.buttonCtrl = false
    end

    self:super("GComponent", "destroy")
end

function M:setList(list)
	self._obj.items = list
end

function M:getList()
	return self._obj.items
end

function M:getSelectedText()
	return self._obj.items[self._obj.selectedIndex]
end

function M:setSelectedIndex(index, call)
	self._obj.selectedIndex = index - 1
	if call then
		self._obj.onChanged:Call()
	end
end

function M:getSelectedIndex()
	return self._obj.selectedIndex + 1
end

function M:getSelectedData()
	if self._dataProvider then
		local index = self:getSelectedIndex()
		return self._dataProvider[index]
	end
end

function M:setText(text)
	self._obj.title = text
end
function M:getText()
	return self._obj.title
end

function M:onChanged(func)
	return self._obj.onChanged:Add(func)
end

function M:setDataProvider(array)
	self._dataProvider = array
end

function M:getDataProvider()
	return self._dataProvider
end

function M:setVisibleItemCount(count)
	self._obj.visibleItemCount = count
end


function M:onClosed(func)
	return self._obj.dropdown.onRemovedFromStage:Add(func)
end

function M:setIcons( icons )
	self._obj.icons = icons
end

function M:GetDropdown()
	return self._obj.dropdown
end
--[[
FairyGUI.PopupDirection
Auto
Up
Down
]]
function M:setPopupDirection(dir)
	self._obj.popupDirection = dir
end


function M:setListDataProvider(data)
	self._listDataProvider = data
	local texts = {}
	for k, v in ipairs(data) do
		table.insert(texts, v.text)
	end
	self._obj.items = texts
end

function M:getListSelectedData()
	if self._listDataProvider then
		local index = self:getSelectedIndex()
		return self._listDataProvider[index]
	end
end


rawset(_G, "GComboBox", M)
