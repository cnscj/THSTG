local M = class("Entity")
--理论上Entity只是一个id而已
function M:ctor()
    self._id = 0
    self._owner = false  --所属世界

    self._components = false
    self._componentArchetype = false

    self._dirtyComponents = false
end
--
function M:getId()
    return self._id
end

function M:getWorld()
    return self._owner
end
--
function M:addComponent(className)

end

function M:removeComponent(className)

end

function M:getComponent(className)

end

function M:replaceComponent(className,comp)
    
end

function M:removeAllComponents()

end

function M:addToWorld(world)
    if not world then return end 

    world:addEntity(self)
end

function M:removeFromWorld()
    if self._owner then
        self._owner:removeEntity(self)
    end
end

function M:dispose()
    self:removeFromWorld()
    self:removeAllComponents()

    ECSManager:recycleEntity(self)
end


return M