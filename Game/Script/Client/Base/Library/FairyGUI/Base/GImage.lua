---@class GImage:GObject
local M = class("GImage", GObject)

function M:ctor(obj)
end

function M:init(obj)
end

--0~1
function M:setPrecent(value)
	self._obj.fillAmount = value
end

-- FlipType.None		0	不翻转
-- FlipType.Horizontal	1	水平翻转
-- FlipType.Vertical	2	垂直翻转
-- FlipType.Both		3	水平+垂直翻转
function M:setFlip(value)
	self._obj.flip = value
end

--[[
fillOrigin
fillClockwise
fillAmount
texture
material
shader
]]

rawset(_G, "GImage", M)

