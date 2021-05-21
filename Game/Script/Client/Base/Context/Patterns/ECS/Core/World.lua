local M = class("World")
local EMPTY_TABLE = {}
--持有所有entity和system和负责收集对应的entity
function M:ctor()
    self.name = "World"

    self._systemsList = {}
    self._entitiesWithId = {}
    self._entitiesWithArchetype = {}

    --TODO:这里应该创建一个ArchetypeCollector,在注册system时添加Filter收集Entity
    --System获取Entity有2中,一种直接作为Update参数,另一种是从World拿
    self._filterArchetypes = {}

    self._entityRemoveQueue = {}
    self._componentRemoveQueue = {}
    self._systemRemoveQueue = {}
end

function M:getEntitiesByArchetype(archetype)
    if not archetype then return end 
    local archetypeKey = archetype:toString()
    local curInfo = self._entitiesWithArchetype[archetypeKey]
    if curInfo then
        return curInfo.entitiesDict
    end
    return EMPTY_TABLE
end

function M:getEntityById(id)
    return self._entitiesWithId[id]
end

--TODO:移除添加操作应该移到帧后进行,否则
--TODO:搜集entity的方式存在疑点
function M:bindEntityComponent(entity,comp)
    if not entity then return end 
    if not comp then return end 
    local className = comp.__cname
    local archetype = ECSManager:getComponentClassArchetype(className)
    if archetype then 
        local archetypeAll = ECSManager:getEntityComponentsArchetype(entity)
        local archetypeAllKey = archetypeAll:toString()  
        local curInfo = self._entitiesWithArchetype[archetypeAllKey]
        if not curInfo then
            curInfo = {}
            curInfo.archetype = archetypeAll
            curInfo.entitiesDict = {}
            curInfo.entitiesList = false
            self._entitiesWithArchetype[archetypeAllKey] = curInfo
        end

        --
        local entityId = entity:getId()
        for _,infoInDict in pairs(self._entitiesWithArchetype) do
            local archetypeInDict = infoInDict.archetype
            if archetypeAll:containAll(archetypeInDict) or archetypeInDict:containAll(archetypeAll) then
                infoInDict.entitiesDict[entityId] = entity
                infoInDict.entitiesList = false
            end
        end
    end
end


function M:unbindEntityComponent(entity,comp)
    if not entity then return end 
    if not comp then return end 
    
    table.insert(self._componentRemoveQueue, {entity = entity,component = comp})
end

function M:_disposeEntityComponent(entity,comp)
    if not entity then return end 
    if not comp then return end 
    local className = comp.__cname
    local archetype = ECSManager:getComponentClassArchetype(className)
    if archetype then 
        local entityId = entity:getId()
        for _,infoInDict in pairs(self._entitiesWithArchetype) do
            local archetypeInDict = infoInDict.archetype 
            if archetypeInDict:containAll(archetype) then
                infoInDict.entitiesDict[entityId] = nil
                infoInDict.entitiesList = false
            end
        end
    end
end

--TODO:
function M:dirtyEntityComponent(entity,comp)
    if not entity then return end 
    if not comp then return end 
    local className = comp.__cname


end

--TODO:应该针对所有组合去添加
function M:bindEntityComponents(entity)
    if not entity then return end 

    local components = ECSManager:getEntityComponents(entity)
    for _,comp in pairs(components) do 
        self:bindEntityComponent(entity,comp)
    end
end

function M:unbindEntityComponents(entity)
    if not entity then return end 

    local components = ECSManager:getEntityComponents(entity)
    for _,comp in pairs(components) do 
        self:unbindEntityComponent(entity,comp)
    end
end

function M:addEntity(entity)
    if not entity then return end 
    entity:removeFromWorld()

    local entityId = entity:getId()
    if not self._entitiesWithId[entityId] then
        entity._owner = self
        self._entitiesWithId[entityId] = entity
        self:bindEntityComponents(entity)
    end
end

function M:removeEntity(entity)
    if not entity then return end 
    table.insert(self._entityRemoveQueue, entity)
end

function M:_disposeEntity(entity)
    if not entity then return end 
    if entity._owner ~= self then return end 

    local entityId = entity:getId()
    if self._entitiesWithId[entityId] then
        self:unbindEntityComponents(entity)
        entity._owner = false
        self._entitiesWithId[entityId] = nil
    end
end

function M:addSystem(system)
    if not system then return end
    if system._owner then 
        system._owner:removeSystem(system)
    end

    system._owner = self
    table.insert(self._systemsList, system)
end

function M:removeSystem(system)
    if not system then return end

    table.insert(self._systemRemoveQueue, system )
end

function M:_disposeSystem(system)
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
   self:_updateSystems(dt)

   self:_purgeEntities(dt)
   self:_purgeSystems(dt)
   self:_purgeSystem(dt)
end

function M:clear()

end

function M:_updateSystems(dt)
    for _,system in ipairs(self._systemsList) do 
        system:update(dt)
    end
end

function M:_purgeSystems(dt)
    for i = #self._systemRemoveQueue , 1, -1 do 
        local system = self._systemRemoveQueue[i]
        self:_disposeSystem(system)
        table.remove(self._systemRemoveQueue, i)
    end
end

function M:_purgeComponents(dt)
    for i = #self._componentRemoveQueue , 1, -1 do 
        local tuple = self._componentRemoveQueue[i]
        local entity = tuple.entity
        local component = tuple.component
        table.remove(self._componentRemoveQueue, i)
    end
end

function M:_purgeEntities(dt)
    for i = #self._entityRemoveQueue , 1, -1 do 
        local entity = self._entityRemoveQueue[i]
        self:_disposeEntity(entity)
        table.remove(self._entityRemoveQueue, i)
    end
end

return M