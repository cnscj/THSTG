---@class GGraph:GObject
local M = class("GGraph", GObject)

--[[
对应编辑器里的图形对象。图形有两个用途，一是用来显示简单的图形，例如矩形等；二是作为一个占位的用途，
可以将本对象替换为其他对象，或者在它的前后添加其他对象，相当于一个位置和深度的占位；还可以直接将内容设置为原生对象。
]]

function M:ctor(obj)
end

function M:init(obj)
end

function M:setColor(color)
    self._obj.color = color
end

-- 用effect或者model
-- function M:setNativeObject(displayObject)
-- 	self._obj:SetNativeObject(displayObject)
-- end

function M:replaceMe(target)
	self._obj:ReplaceMe(target)
end

function M:drawRect(width, height, lineSize, lineColor, fillColor)
	self._obj:DrawRect(width, height, lineSize, lineColor, fillColor)
end

rawset(_G, "GGraph", M)
