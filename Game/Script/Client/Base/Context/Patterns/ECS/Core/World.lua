local M = class("World")
local EMPTY_TABLE = {}
--持有所有entity和system和负责收集对应的entity
function M:ctor(name)
    self.name = name or "World"

    self._systemsList = {}
    self._systemsExInfo = {}
    self._entitiesWithId = {}
    self._entitiesWithArchetype = {}

    self._purgeDirtyQueue = {}
end

function M:getEntitiesByArchetype(archetype)
    if not archetype then return end 
    local archetypeKey = archetype:toString()
    local curInfo = self._entitiesWithArchetype[archetypeKey]
    if curInfo then
        return curInfo.updateEntitiesDict
    end
    return EMPTY_TABLE
end

function M:getEntityById(id)
    return self._entitiesWithId[id]
end

function M:dirtyEntityComponent(entity,comp)
    if not entity then return end 
    if not comp then return end 

    local className = comp.__cname
    local archetype = ECSManager:getComponentClassArchetype(className)
    self:_modifyEntityComponentArchetype(entity,archetype)
end

function M:bindEntityComponent(entity,comp)
    if not entity then return end 
    if not comp then return end 

    local className = comp.__cname
    local archetype = ECSManager:getComponentClassArchetype(className)
    self:_addEntityComponentArchetype(entity,archetype)
end

function M:unbindEntityComponent(entity,comp)
    if not entity then return end 
    if not comp then return end 
    
    local className = comp.__cname
    local archetype = ECSManager:getComponentClassArchetype(className)
    self:_removeEntityComponentArchetype(entity,archetype)
end

--TODO:这里把
function M:_createEntitiesArchetypeData(archetype)
    local curInfo = {}
    curInfo.archetype = archetype
    curInfo.updateEntitiesDict = {}
    curInfo.modifyEntitiesDict = {first = {},second = {}}
    return curInfo
end

function M:_bindEntityComponents(entity)
    if not entity then return end 

    local archetypeAll = ECSManager:getEntityComponentsArchetype(entity)
    self:_addEntityComponentArchetype(entity,archetypeAll)
    self:_modifyEntityComponentArchetype(entity,archetypeAll)
end

--TODO:移除应该放到帧后
function M:_unbindEntityComponents(entity)
    if not entity then return end 

    local archetypeAll = ECSManager:getEntityComponentsArchetype(entity)
    self:_modifyEntityComponentArchetype(entity,archetypeAll)
    self:_removeEntityComponentArchetype(entity,archetypeAll)
end

function M:_bindSystemComponents(system)
    if not system then return end 

    local listenedComponentsArchetype = self._systemsExInfo[system].listenedComponentsArchetype
    local listenedComponentsArchetypeKey = listenedComponentsArchetype:toString()
    local curInfo = self._entitiesWithArchetype[listenedComponentsArchetypeKey]
    if not curInfo then
        curInfo = self:_createEntitiesArchetypeData(listenedComponentsArchetype)
        self._entitiesWithArchetype[listenedComponentsArchetypeKey] = curInfo
    end
end

function M:_addEntityComponentArchetype(entity,archetype)
    if not entity then return end 
    if not archetype then return end 
    
    local archetypeKey = archetype:toString()
    local curInfo = self._entitiesWithArchetype[archetypeKey]
    if not curInfo then
        curInfo = self:_createEntitiesArchetypeData(archetype)
        self._entitiesWithArchetype[archetypeKey] = curInfo

        --这里新的archetype集合需要把以前符合条件的entity也添加上去
        for _,entityInDict in pairs(self._entitiesWithId) do 
            local entityIdInDict = entityInDict:getId()
            local entityComponentsArchetype = ECSManager:getEntityComponentsArchetype(entityInDict)
            if entityComponentsArchetype:containAll(archetype) then
                curInfo.updateEntitiesDict[entityIdInDict] = entityInDict
            end
        end
    end

    --其他archetype可能不止一个,因此用遍历让其他符合条件的集合把自己也加上
    local entityId = entity:getId()
    for _,infoInDict in pairs(self._entitiesWithArchetype) do
        local archetypeInDict = infoInDict.archetype
        if archetype:containAll(archetypeInDict) then
            infoInDict.updateEntitiesDict[entityId] = entity
        end
    end
end

function M:_removeEntityComponentArchetype(entity,archetype)
    if not entity then return end 
    if not archetype then return end 
 
    local entityId = entity:getId()
    for _,infoInDict in pairs(self._entitiesWithArchetype) do
        local archetypeInDict = infoInDict.archetype 
        if archetype:containAll(archetypeInDict) then
            infoInDict.updateEntitiesDict[entityId] = nil
        end
    end
end

--TODO:防止重复dirty
--这里应该先记录一个总的archetype,等update之前在收集
function M:_modifyEntityComponentArchetype(entity,archetype)
    if not entity then return end 
    if not archetype then return end 

    for i = #self._systemsList,1,-1 do
        local system = self._systemsList[i]

        local listenedComponentsArchetype = self._systemsExInfo[system].listenedComponentsArchetype
        if archetype:containAll(listenedComponentsArchetype) then
            local entityId = entity:getId()
            local listenedComponentsArchetypeKey = listenedComponentsArchetype:toString()
            local modifyEntities = self._entitiesWithArchetype[listenedComponentsArchetypeKey].modifyEntitiesDict.first
            modifyEntities[entityId] = entity
        end
    end

end

function M:addEntity(entity)
    if not entity then return end 
    entity:removeFromWorld()

    local entityId = entity:getId()
    if not self._entitiesWithId[entityId] then
        entity._owner = self
        self._entitiesWithId[entityId] = entity
        self:_bindEntityComponents(entity)
    end
end

function M:removeEntity(entity)
    if not entity then return end 
    if entity._owner ~= self then return end 

    local entityId = entity:getId()
    if self._entitiesWithId[entityId] then
        self:_unbindEntityComponents(entity)
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

    local listenedComponentsArchetype = ECSManager:getArchetypeByComponentClassArray(system._listenedComponents)
    self._systemsExInfo[system] = {
        listenedComponentsArchetype = listenedComponentsArchetype
    }
    self:_bindSystemComponents(system)
end

function M:removeSystem(system)
    if not system then return end

    for i = #self._systemsList,1,-1 do
        if system == self._systemsList[i] then

            system._owner = false
            table.remove(self._systemsList, i)

            self._systemsExInfo[system] = nil
            break
        end
    end
end

function M:update(dt)
    self:_updateSystems(dt)
    self:_purgeSystems(dt)
end

function M:clear()

end

function M:_updateSystems(dt)
    for _,system in ipairs(self._systemsList) do 
        system:update(dt)

        --收集system中所监听所变化的Entity
        local listenedComponentsArchetype = self._systemsExInfo[system].listenedComponentsArchetype
        local listenedComponentsArchetypeKey = listenedComponentsArchetype:toString()

        --如果在update中的时候modify了,就不需要清除
        --这里应该采用2个Cache交替变换的做法
        if not self._purgeDirtyQueue[listenedComponentsArchetypeKey] then
            local dirtyFirstDict = self._entitiesWithArchetype[listenedComponentsArchetypeKey].modifyEntitiesDict.first
            local dirtySecondDict = self._entitiesWithArchetype[listenedComponentsArchetypeKey].modifyEntitiesDict.second
            self._entitiesWithArchetype[listenedComponentsArchetypeKey].modifyEntitiesDict.first = dirtySecondDict
            self._entitiesWithArchetype[listenedComponentsArchetypeKey].modifyEntitiesDict.second = dirtyFirstDict  
        end

        local dirtyEntities = self._entitiesWithArchetype[listenedComponentsArchetypeKey].modifyEntitiesDict.second
        if dirtyEntities and next(dirtyEntities) then
            self._purgeDirtyQueue[listenedComponentsArchetypeKey] = listenedComponentsArchetype
            system:modifyUpdate(dirtyEntities)
        end
    end
end

function M:_purgeSystems(dt)
    for key,_ in pairs(self._purgeDirtyQueue) do 
        self._entitiesWithArchetype[key].modifyEntitiesDict.second = {}
        self._purgeDirtyQueue[key] = nil
    end
end

return M