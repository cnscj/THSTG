---@class GSliderInt
local M = class("GSliderInt", GComponent)

function M:ctor(obj)
	self._type = "int"
	self._minNum = 0
	self._maxNum = 1
	self._preNum = false
	self._intNum = false
	self._changedFunc = false
end

function M:init(obj)
end
function M:destroy()
    self._changedFunc = false

    self:super("GComponent", "destroy")
end

function M:setText(text)
end

-- gsliderInt 不要用 __setValue __getValue
function M:__setValue(value, call)
	self._obj.value = value
	self._intNum = value + self._minNum
	if call then
		self._obj.onChanged:Call()
	end
end
function M:__getValue()
	return self._obj.value
end

function M:setIntValue(value, call)
	self._intNum = value 
	self._obj.value = value - self._minNum
	if call then
		self._obj.onChanged:Call()
	end
end

function M:getIntValue()
	return self._intNum 
end


function M:toMin(call)
	self:__setValue(0, call)
end

function M:toMax(call)
	self:__setValue(self._maxNum-self._minNum, call)
end

-- 设置整数，不可逆
function M:setRange(minNum, maxNum)
	if self._minNum == self._maxNum then
		printWarning("最大值和最小值不能相同")
		return
	end
	self._minNum = minNum
	self._maxNum = maxNum
	self._obj.max = self._maxNum-self._minNum
	self:__setValue(0)
	self:setChanged(function ()
		local value = math.ceil(self._obj.value - 0.5)
		if value <= 0 then
			value = 0
		end
		self:__setValue(value)
		if value ~= self._preNum then
			self._preNum = value
			if self._changedFunc then
				self._changedFunc(self._intNum)
			end
		end
	end)
end

function M:setChangedFunc(func)
	self._changedFunc = func
end


function M:changeOnClick(bool)
	-- 点击滚动条改变进度，默认是true
	self._obj.changeOnClick = bool
end

function M:onChanged(func)
	self._obj.onChanged:Add(func)
end

function M:setChanged(func)
	self._obj.onChanged:Set(func)
end

function M:getMax()
	return self._maxNum
end

function M:getMin()
	return self._minNum
end

function M:getPercent()
	return self._obj.value / self._obj.max
end


rawset(_G, "GSliderInt", M)
