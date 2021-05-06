local M = class("World")
--持有所有entity和system和负责收集对应的entity
function M:ctor()
    self._systemsList = {}
    self._entitiesDict = {}

    self._archetypeManager = ArchetypeManager.new()
end

function M:getEntities(...)
    return self._archetypeManager:getEntities(...)
end

function M:addEntity(entity)
    if not entity then return end 
    entity:removeFromWorld()

    local entityId = entity:getId()
    if not self._entitiesDict[entityId] then
        entity._owner = self
        self._entitiesDict[entityId] = entity
    end
end

function M:removeEntity(entity)
    if not entity then return end 

    local entityId = entity:getId()
    if self._entitiesDict[entityId] then
        entity._owner = false
        self._entitiesDict[entityId] = nil
    end
end

function M:addSystem(system)
    if not system then return end
    if system._owner ~= false then 
        system._owner:removeSystem(system)
    end

    system._owner = self
    table.insert(self._systemsList, system)
end

function M:removeSystem(system)
    if not system then return end
    if not system._owner ~= self then return end

    for i = #self._systemsList,1,-1 do
        if system == self._systemsList[i] then

            system._owner = false
            table.remove(self._systemsList, i)
            break
        end
    end
end

function M:update(dt)
    for _,system in ipairs(self._systemsList) do 
        system:update(dt)
    end 
end

return M