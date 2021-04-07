---@class GSlider
local M = class("GSlider", GComponent)

function M:ctor(obj)
end

function M:init(obj)
end


function M:setValue(value, call)
	self._obj.value = value
	if call then
		self._obj.onChanged:Call()
	end
end

function M:getValue()
	if self._obj.wholeNumbers then
		return math.floor(self._obj.value)
	end
	return self._obj.value
end

function M:setMin(min)
	self._obj.min = min
end
function M:getMin()
	return self._obj.min
end

function M:setMax(max)
	self._obj.max = max
end
function M:getMax()
	return self._obj.max
end

function M:setValueMax(value, max)
	self._obj.value = value
	self._obj.max = max
end

function M:setPercent(percent)
	local val = percent * self._obj.value
	self:setValue(val)
end

function M:getPercent()
	return self._obj.value / self._obj.max
end

function M:onChanged(func)
	self._obj.onChanged:Set(func)
end


function M:setRange(min, max)
	self._obj.min = min
	self._obj.max = max
end


function M:changeOnClick(change)
	-- 点击滚动条改变进度，默认是true
	self._obj.changeOnClick = change
end


function M:setWholeNumbers(bool)
	self._obj.wholeNumbers = bool
end

rawset(_G, "GSlider", M)
