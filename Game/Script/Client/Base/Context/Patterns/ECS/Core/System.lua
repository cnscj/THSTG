local M = class("System")
function M:ctor()
    self._owner = false

    self._listenedComponents = false
end

function M:getEntities(...)
    --可能有三种情况,一种是Archetype,另一种是classArray,另一种全是形参
    if not self._owner then return table.empty end

    local componentsArchetype = false
    local arg = select(1,...)
    if arg then
        if arg.__cname then
            componentsArchetype = arg
        else
            componentsArchetype = ECSManager:getArchetypeByComponentClasses(...)
        end
        return self._owner:getEntitiesByArchetype(componentsArchetype)
    end
    return table.empty
end


function M:removeFromWorld()
    if self._owner then
        self._owner:removeSystem(self)
    end
end

function M:update(dt)
 
end

function M:modifyUpdate(entities)

end
--

return M