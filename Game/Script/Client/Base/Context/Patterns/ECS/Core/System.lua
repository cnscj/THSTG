local M = class("System")
local EMPTY_TABLE = {}
function M:ctor()
    self._owner = false

    self._componentsArchetype = false
end

function M:getEntities(arg)
    --可能有两种情况,一种是Archetype,另一种是className
    if not self._owner then return end
    if type(arg) == "table" then
        if arg.__cname == "Archetype" then
            return self._owner:getEntitiesByArchetype(arg)
        else
            local componentsArchetype = self._componentsArchetype or Archetype.new()
            componentsArchetype:clear()

            for _,className in ipairs(arg) do 
                local archetype = ECSManager:getComponentClassArchetype(className)
                if not archetype then return EMPTY_TABLE end

                componentsArchetype:add(archetype)
            end
            return self._owner:getEntitiesByArchetype(componentsArchetype)
        end
    end
    
    return EMPTY_TABLE
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