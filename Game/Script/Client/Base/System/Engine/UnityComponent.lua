---@class UnityComponent
local M = class("UnityComponent")

function M:ctor(owner)
	self._type = false
	self._entity = false  --Unity Component instance
	self._owner = owner   --Node
	
	self._enabled = -1
end

function M:getType()
	return self._type
end

function M:getEntity()
	return self._entity
end

function M:getOwner()
	return self._owner
end

function M:instantiate()
	local entity = self._owner:getEntity():GetComponent(self._type)
	if not entity then
		entity = self._owner:getEntity():AddComponent(self._type) or false
	end

	self._entity = entity

	if self._enabled ~= -1 then
		local check = false
		if self._enabled == 1 then check = true end
		self._entity:SetEnabled(check)
	end
	self:reset()
end

function M:setEnabled(enabled)
	if type(enabled) == "number" then
		if enabled == 1 then enabled = true 
		else enabled = false end
	end
	if self._entity then	
		self._entity:SetEnabled(enabled)
	else
		if enabled then 
			self._enabled = 1
		else
			self._enabled = 0
		end
	end
end

function M:onDestroy()
	self._entity = false
end

--- 进对象池时调用
function M:clear()
end

--- 出对象池时或者init调用
function M:reset()
end

rawset(_G, "UnityComponent", M)