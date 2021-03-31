local M = class("ECSManager")
local OBJECT_POOL_CONFIG = {        --对象池配置
    [Entity] = {maxCount = -1},
}

function M:ctor()
    self._worlds = {}
    self._entityIdDict = {}
    self._compCName2TypeDict = {}
    self._entityIncreaseId = 0
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
        end
    end
    return pool
end

function M:_getNewEntityId()
    self._entityIncreaseId = self._entityIncreaseId + 1
    return self._entityIncreaseId 
end

function M:_getEntityPool()
    local entityPool = self:_getOrCreatePool(Entity)
    return entityPool
end
--
function M:registerComponent(Type)
    self:addComponentTypeByType(Type)
end

function M:getOrCreateComponent(typeName)
    local Type = self:getComponentTypeByName(typeName)
    local pool = self:_getOrCreatePool(Type)
    local comp = pool:getOrCreate()
    return comp
end

function M:recycleComponent(comp)
    local cname = comp.__cname
    local Type = self:getComponentTypeByName(cname)
    local pool = getPool(Type) 
    if pool then pool:release(comp) end
end

function M:getOrCreateEntity()
    local entityPool = self:_getEntityPool()
    local entity = entityPool:getOrCreate()
    entity._id = self:_getNewEntityId()

    self._entityIdDict[entity._id] = entity
    
    return entity
end

function M:getEntityById(entityId)
    return self._entityIdDict[entityId]
end

function M:recycleEntity(entity)
    local entityId = entity._id
    local entityPool = self:_getEntityPool()

    entity._id = 0
    entityPool:release(entity)
    self._entityIdDict[entityId] = nil

end

--
function M:addWorld(world)
    table.insert(self._worlds, world)
end
---
function M:getComponentTypeByName(cname)
    return self._compCName2TypeDict[cname]
end

function M:addComponentTypeByType(Type)
    local cname = Type.cname
    if not M:getComponentTypeByName(cname) then
        self._compCName2TypeDict[cname] = Type
    end
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