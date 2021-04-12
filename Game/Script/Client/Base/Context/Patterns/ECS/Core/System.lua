local M = class("System")

function M:ctor()
    self._owner = false
end

function M:getEntities(...)
    if not self._owner then return end
    return self._owner:getEntities(...)
    
end

function M:update(dt)

end

return M