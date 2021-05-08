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
        local componentsArchetype = self._componentsArchetype or Archetype.new()
        componentsArchetype:clear()

        for _,className in ipairs(arg) do 
            local archetype = ECSManager:getComponentClassArchetype(className)
            if not archetype then return EMPTY_TABLE end

            componentsArchetype:add(archetype)
        end
        return self._owner:getEntitiesByArchetype(componentsArchetype)
    elseif type(arg) == "userdata" then
        return self._owner:getEntitiesByArchetype(arg)
    end
    
    return EMPTY_TABLE
end


function M:update(dt)

end
--

return M