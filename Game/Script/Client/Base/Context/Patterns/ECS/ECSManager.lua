local M = class("ECSManager")
local OBJECT_POOL_CONFIG = {        --对象池配置
    [ECS.Entity] = {maxCount = -1, minCount = 20},
}

function M:ctor()
    self._worlds = {}
    self._entities = {}

    self._componentIds = 0
    self._componentClassExInfo = {}

    self._entityIds = 0
end

--
function M:registerComponent(Cls)
    if not Cls then return end 
    if not Cls.isTypeOf("Component") then return end 

    --注册额外的信息
    local cname = Cls.cname
    if not self._componentClassExInfo[cname] then
        --给每个component分配一个id作为唯一标识
        local compId = self:_getNewComponentId()
        local archetype = Archetype.new(compId)

        self._componentClassExInfo[cname] = {
            id = compId,
            cname = cname,
            cls = Cls,
            archetype = archetype
        }
    end
end

function M:createComponent(className)
    local pool = self:_tryGetComponentPool(className)
    local comp = false
    if pool then
        comp = pool:getOrCreate()
    end
    return comp
end

function M:_getNewComponentId()
    self._componentIds = self._componentIds + 1
    return self._componentIds
end

function M:_getComponentClassByName(cname)
    local exInfo = self._componentClassExInfo[cname]
    if exInfo then
        return exInfo.type
    end
end

function M:_tryGetComponentPool(className)
    local Cls = self:_getComponentClassByName(className)
    local componentPool = false
    if Cls then
        componentPool = self:_getOrCreatePool(Cls)
    end
    return componentPool
end

--
function M:_recycleComponent(comp)
    local cname = comp.__cname
    local pool = self:_tryGetComponentPool(cname) 
    if pool then pool:release(comp) end
end


--
function M:createEntity()
    local entityPool = self:_getEntityPool()
    local entity = entityPool:getOrCreate()
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

--
function M:createWorld()

end

function M:addWorld(world)
    table.insert(self._worlds, world)
end
---

function M:_getPool(Cls)
    local pool = ObjectPoolManager:getPool(Cls)
    return pool
end

function M:_getOrCreatePool(Cls)
    local pool = ObjectPoolManager:getPool(Cls)
    if not pool then
        pool = ObjectPoolManager:createPool(Cls)
        local poolConfig = OBJECT_POOL_CONFIG[Cls]
        if poolConfig then
            pool.maxCount = poolConfig.maxCount
            pool.minCount = poolConfig.minCount
        end
    end
    return pool
end

function M:_getNewEntityId()
    self._entityIds = self._entityIds + 1
    return self._entityIds 
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