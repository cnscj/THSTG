---@class UnityComponent
local M = class("UnityComponent")

function M:ctor(obj)
	self._obj = obj
end

function M:getObj()
    return self._obj
end

function M:isEnabled( ... )

end


rawset(_G, "UnityComponent", M)