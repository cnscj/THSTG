local M = class("Entity")

function M:ctor()
    self._id = 0
    self._owner = false  --所属世界

    self._components = false
    self._archetype = false
end

function M:addComponent(typeName)

end

function M:removeComponent(typeName)

end

function M:getComponent(typeName)

end

function M:replaceComponent(typeName,comp)
    
end

function M:clear()

end

function M:removeFromWorld()
    if self._owner then
        self._owner:removeEntity()
    end
end

return M