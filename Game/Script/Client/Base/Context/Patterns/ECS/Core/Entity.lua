local M = class("Entity")
--理论上Entity只是一个id而已
function M:ctor()
    self._id = 0
    self._owner = false  --所属世界
end
--
function M:getId()
    return self._id
end

function M:getWorld()
    return self._owner
end

function M:addComponent(compCls)
    return ECSManager:addEntityComponent(self,compCls)
end

function M:removeComponent(compCls)
    ECSManager:removeEntityComponent(self,compCls)
end

function M:getComponent(compCls)
   return ECSManager:getEntityComponent(self,compCls)
end

function M:replaceComponent(newComp)
    ECSManager:replaceEntityComponent(self,newComp)
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

    ECSManager:disposeEntity(self)
end

function M:clear()
    self._id = 0
    self._owner = false
end

return M