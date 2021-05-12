local Entity = ECS.Entity
local Component = ECS.Component
local OBJECT_POOL_CONFIG = {        --对象池配置
    [Entity] = {maxCount = -1, minCount = 20},
}
local M = class("ECSManager")


function M:ctor()
    self._worlds = {}
    self._entities = {}

    self._componentIds = 0
    self._componentClassExInfo = {}

    self._entityIds = 0
    self._entityChunkData = {}

    --注册一个轮询函数
    self._updateFunction = function ( ... )
        self:update(CSharp.Time.deltaTime)
    end

    --注册更新
    CSharp.MonoManagerIns:AddUpdateListener(self._updateFunction)
end

--
function M:registerComponentClass(cls)
    if not cls then return end 
    if not cls.isTypeOf(Component.cname) then return end
    
    --注册额外的信息
    local cname = cls.cname
    if not self._componentClassExInfo[cname] then
        --给每个component分配一个id作为唯一标识
        local compId = self:_getNewComponentId()
        local archetype = Archetype.new(compId - 1)

        self._componentClassExInfo[cname] = {
            id = compId,
            cname = cname,
            cls = cls,
            archetype = archetype
        }
    end
end

function M:isComponentClassRegistered(className)
    return self._componentClassExInfo[className]
end

function M:createComponent(className)
    local pool = self:_tryGetComponentPool(className)
    local comp = false
    if pool then
        comp = pool:getOrCreate()
        comp:clear()
    end
    return comp
end

function M:getComponentClassArchetype(className)
    if not className then return false end 
    local classExInfo = self._componentClassExInfo[className]
    if classExInfo then
        return classExInfo.archetype
    end
end

function M:recycleComponent(comp)
    local cname = comp.__cname
    local pool = self:_tryGetComponentPool(cname) 
    if pool then 
        pool:release(comp) 
    end
end

function M:_getNewComponentId()
    self._componentIds = self._componentIds + 1
    return self._componentIds
end

function M:_getComponentClassByName(cname)
    local exInfo = self._componentClassExInfo[cname]
    if exInfo then
        return exInfo.cls
    end
    return false
end

function M:_tryGetComponentPool(className)
    local cls = self:_getComponentClassByName(className)
    local componentPool = self:_getOrCreatePool(cls)
    return componentPool
end


--
function M:createEntity()
    local entityPool = self:_getEntityPool()
    local entity = entityPool:getOrCreate()
    entity:clear()
    
    entity._id = self:_getNewEntityId()
    self._entities[entity._id] = entity
    return entity
end

function M:getEntityById(entityId)
    return self._entities[entityId]
end

function M:recycleEntity(entity)
    local entityId = entity._id
    self._entities[entityId] = nil
    entity._id = 0

    local entityPool = self:_getEntityPool()
    entityPool:release(entity)
end

function M:_getNewEntityId()
    self._entityIds = self._entityIds + 1
    return self._entityIds 
end
--

-- function M:addEntityComponent(entity,componentName)
--     if not entity then return end 
--     if not componentName then return end 

--     local chunkData = self:getEntityChunkData(entity)
--     if chunkData then
--         local componentArchetype = self:getComponentClassArchetype(componentName)
--         if componentArchetype then
--             chunkData.componentsArchetype:add(componentArchetype)
--             chunkData.components[componentName] = component
--         end
--     end
-- end

-- function M:removeEntityComponent(entity,componentName)
--     if not entity then return end 
--     if not componentName then return end 

--     local chunkData = self:getEntityChunkData(entity)
--     if chunkData then
--         local componentArchetype = self:getComponentClassArchetype(componentName)
--         if componentArchetype then
--             chunkData.componentsArchetype:del(componentArchetype)
--             chunkData.components[componentName] = nil
--         end
--     end
-- end

-- function M:replaceEntityComponent(entity,comp)
--     if not entity then return end 
--     if not comp then return end 

--     local entityId = entity:getId()
--     local componentName = comp.__cname 

-- end

-- function M:getEntityComponent(entity,className)
--     if not entity then return end 
--     if not componentName then return end 

--     local chunkData = self:getEntityChunkData(entity)
--     if chunkData then
--         return chunkData.components[componentName]
--     end
-- end

-- function M:getEntityChunkData(entity)
--     local entityId = entity:getId()
--     return self._entityChunkData[entityId]
-- end

--
function M:addWorld(world)
    table.insert(self._worlds, world)
end

function M:removeWorld(world)
    if self._worlds then
        for i,worldInList in ipairs(self._worlds) do 
            if world == worldInList then
                table.remove( list, i )
                break
            end
        end
    end
end
---

function M:_getPool(cls)
    local pool = ObjectPoolManager:getPool(cls)
    return pool
end

function M:_getOrCreatePool(cls)
    local pool = ObjectPoolManager:getPool(cls)
    if not pool then
        pool = ObjectPoolManager:createPool(cls)
        local poolConfig = OBJECT_POOL_CONFIG[cls]
        if poolConfig then
            pool.maxCount = poolConfig.maxCount
            pool.minCount = poolConfig.minCount
        end
    end
    return pool
end


function M:_getEntityPool()
    local entityPool = self:_getOrCreatePool(Entity)
    return entityPool
end


--
function M:update(dt)
    for _,world in pairs(self._worlds) do 
        world:update(dt)
    end
end

----
function M:clear()

end

rawset(_G, "ECSManager", false)
ECSManager = M.new()