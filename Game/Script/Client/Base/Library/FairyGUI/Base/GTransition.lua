---@class GTransition
local M = class("GTransition")

function M:ctor(obj)
    self._obj = obj or false
end

function M:init(obj)
end
function M:destroy()
    self._obj = false
end

function M:getObj()
    return self._obj
end


rawset(_G, "GTransition", M)
