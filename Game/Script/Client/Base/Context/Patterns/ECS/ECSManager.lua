local M = class("ECSManager")
local OBJECT_POOL_CONFIG = {        --对象池配置
    [ECS.Entity] = {maxCount = -1, minCount = 20},
}

function M:ctor()
    self._worlds = {}
    self._entities = {}


    self._compCName2TypeDict = {}
    self._entityUID = 0
end

--
function M:registerComponent(Type)
    --TODO:给每个component分配一个2^n次方作为flag,之后查找时只要|下即可
    if not Type then return end 
    if not Type.isTypeOf("Component") then return end 

    self:_addComponentTypeByType(Type)
end

function M:createComponent(typeName)
    local pool = self:_tryGetComponentPool(typeName)
    local comp = false
    if pool then
        comp = pool:getOrCreate()
    end
    return comp
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
function M:_getComponentTypeByName(cname)
    return self._compCName2TypeDict[cname]
end

function M:_addComponentTypeByType(Type)
    local cname = Type.cname
    if not M:_getComponentTypeByName(cname) then
        self._compCName2TypeDict[cname] = Type
    end
end

function M:_getPool(Type)
    local pool = ObjectPoolManager:getPool(Type)
    return pool
end

function M:_getOrCreatePool(Type)
    local pool = ObjectPoolManager:getPool(Type)
    if not pool then
        pool = ObjectPoolManager:createPool(Type)
        local poolConfig = OBJECT_POOL_CONFIG[Type]
        if poolConfig then
            pool.maxCount = poolConfig.maxCount
            pool.minCount = poolConfig.minCount
        end
    end
    return pool
end

function M:_getNewEntityId()
    self._entityUID = self._entityUID + 1
    return self._entityUID 
end

function M:_getEntityPool()
    local entityPool = self:_getOrCreatePool(Entity)
    return entityPool
end

function M:_tryGetComponentPool(typeName)
    local Type = self:_getComponentTypeByName(typeName)
    local componentPool = false
    if Type then
        componentPool = self:_getOrCreatePool(Type)
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