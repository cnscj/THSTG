local M = class("World")
--持有所有entity和system和负责收集对应的entity
function M:ctor()
    self._systems = {}
    self._entities = {}

    self._archetypeManager = false
end

function M:getEntities(...)

end

function M:addEntity(entity)

end

function M:removeEntity(entity)

end

function M:addSystem(system)
    system._owner = self

    table.insert(self._systems, system)
end

function M:removeSystem(system)
    for i = #self._systems,1,-1 do
        if system == self._systems[i] then

            system._owner = false
            table.remove(self._systems, i)
        end
    end
end

function M:update(dt)
    for _,system in ipairs(self._systems) do 
        system:update()
    end 
end

rawset(_G, "World", false)
World = M