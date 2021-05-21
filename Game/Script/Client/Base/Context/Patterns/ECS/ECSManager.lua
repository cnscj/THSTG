local Entity = ECS.Entity
local Component = ECS.Component
local Archetype = Archetype
local EMPTY_TABLE = {}
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

function M:getArchetypeByComponentsClass( ... )
    local argsNum = select("#", ...)
    local finalArchetype = Archetype.new()
    for i = 1, argsNum do
        local arg = select(i, ...)
        local clsName = arg.cname
        local archetype = self:getComponentClassArchetype(clsName)
        if not archetype then
            finalArchetype:clear()
            return finalArchetype
        end
        finalArchetype:add(archetype)
    end
    return finalArchetype
end

function M:recycleComponent(comp)
    if not comp then return end

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

function M:addEntityComponent(entity,compCls)
    if not entity then return end 
    if not compCls then return end 

    local chunkData = self:getEntityData(entity) or self:createEntityData(entity)
    if chunkData then
        local componentName = compCls.cname
        local componentArchetype = self:getComponentClassArchetype(componentName)
        if componentArchetype then
            local component = self:createComponent(componentName)
            chunkData.componentsArchetype:add(componentArchetype)
            chunkData.components[componentName] = component

            local world = entity:getWorld()
            if world then world:bindEntityComponent(self,component) end
        end
    end
end

function M:removeEntityComponent(entity,compCls)
    if not entity then return end 
    if not compCls then return end 

    local chunkData = self:getEntityData(entity)
    if chunkData then
        local componentName = compCls.cname
        local componentArchetype = self:getComponentClassArchetype(componentName)
        if componentArchetype then
            local component = chunkData.components[componentName]
            local world = entity:getWorld()
            if world then world:unbindEntityComponent(self,component) end

            chunkData.componentsArchetype:del(componentArchetype)
            chunkData.components[componentName] = nil

            self:recycleComponent(component)
        end
    end
end

function M:replaceEntityComponent(entity,newComp)
    if not entity then return end 
    if not newComp then return end 

    local entityId = entity:getId()
    local componentName = newComp.__cname 
    local componentArchetype = self:getComponentClassArchetype(componentName) or self:createEntityData(entity)
    if componentArchetype then
        local chunkData = self:getEntityData(entity)
        if chunkData then
            if componentArchetype then
                local oldComp = chunkData.components[componentName]

                chunkData.componentsArchetype:add(componentArchetype)
                chunkData.components[componentName] = newComp
    
                self:recycleComponent(oldComp)
            end
        end
    end 
end

function M:getEntityComponent(entity,compCls)
    if not entity then return end 
    if not compCls then return end 

    local chunkData = self:getEntityData(entity)
    if chunkData then
        local componentName = compCls.cname
        return chunkData.components[componentName]
    end
end

function M:getEntityComponents(entity)
    if not entity then return end 
    local chunkData = self:getEntityData(entity)
    return chunkData and chunkData.components or EMPTY_TABLE
end

function M:getEntityComponentsArchetype(entity)
    if not entity then return end 
    local chunkData = self:getEntityData(entity)
    return chunkData and chunkData.componentsArchetype or Archetype.Empty
end

function M:getEntityData(entity)
    if not entity then return end 
    local entityId = entity:getId()
    return self._entityChunkData[entityId]
end

function M:createEntityData(entity)
    if not entity then return end 
    local entityId = entity:getId()
    local chunkData = {
        componentsArchetype = Archetype.new(),
        components = {},
    }
    self._entityChunkData[entityId] = chunkData
    return chunkData
end

function M:removeAllEntityComponents(entity)
    if not entity then return end 
    local entityId = entity:getId()
    local world = entity:getWorld()

    if self._owner then
        self._owner:unbindEntityComponents(self)
    end

    --回收所有Component
    local components = self:getEntityComponents(entity)
    for _, comp in pairs(components) do 
        self:recycleComponent(comp)
    end
    self._entityChunkData[entityId] = nil
end

--
function M:addWorld(world)
    if world then
        table.insert(self._worlds, world)
    end
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