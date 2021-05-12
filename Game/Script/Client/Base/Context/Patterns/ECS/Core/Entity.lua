local M = class("Entity")
--理论上Entity只是一个id而已
function M:ctor()
    self._id = 0
    self._owner = false  --所属世界

    --XXX:之后转移到ECSManager的作为Chunk数据
    self._components = {}
    self._componentsArchetype = Archetype.new()
end
--
function M:getId()
    return self._id
end

function M:getWorld()
    return self._owner
end

function M:addComponent(className)
    if not self._components[className] then
        local component = ECSManager:createComponent(className)
        if component then
            local archetype = ECSManager:getComponentClassArchetype(className)
            self._componentsArchetype:add(archetype)
            self._components[className] = component            
            
            if self._owner then self._owner:bindEntityComponent(self,className) end
        end
    end
end

function M:removeComponent(className)
    local component = self._components[className]
    if component then
        if self._owner then self._owner:unbindEntityComponent(self,className) end

        local archetype = ECSManager:getComponentClassArchetype(className)
        self._componentsArchetype:del(archetype)
        self._components[className] = nil
    end

end

function M:getComponent(className)
    return self._components[className]
end

function M:getComponents()
    return self._components
end

function M:getComponentsArchetype()
    return self._componentsArchetype
end

function M:replaceComponent(newComp,className)
    if not newComp then
        if className then
            self:removeComponent(className) 
        end
        return
    end

    className = className or newComp.__cname
    if ECSManager:isComponentClassRegistered(className) then
        local oldComp = self:getComponent()
        if oldComp then
            ECSManager:recycleComponent(oldComp)
        end
        self._components[className] = newComp

        if self._owner then self._owner:dirtyEntityComponent(self,className) end
    end
end

function M:removeAllComponents()
    if self._owner then
        self._owner:unbindEntityComponents(self)
    end

    self._components = {}
    self._componentsArchetype:clear()

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

function M:clear()
    self._id = 0
    self._owner = false
    self._components = {}
    self._componentsArchetype:clear()
end

return M