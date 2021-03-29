local M = {}
local OBJECT_POOL_CONFIG = {        --对象池配置
    [Entity] = {maxCount = -1},
}

local _worlds = {}
local _entityIdDict = {}
local _compCName2TypeDict = {}
local _entityIncreaseId = 0

local function getPool(Type)
    local pool = ObjectPoolManager.getPool(Type)
    return pool
end

local function getOrCreatePool(Type)
    local pool = ObjectPoolManager.getPool(Type)
    if not pool then
        pool = ObjectPoolManager.createPool(Type)
        local poolConfig = OBJECT_POOL_CONFIG[Type]
        if poolConfig then
            pool.maxCount = poolConfig.maxCount
        end
    end
    return pool
end

local function getNewEntityId()
    _entityIncreaseId = _entityIncreaseId + 1
    return _entityIncreaseId 
end

local function getEntityPool()
    local entityPool = getOrCreatePool(Entity)
    return entityPool
end
--
function M.registerComponent(Type)
    M.addComponentTypeByType(Type)
end

function M.getOrCreateComponent(typeName)
    local Type = M.getComponentTypeByName(typeName)
    local pool = getOrCreatePool(Type)
    local comp = pool:getOrCreate()
    return comp
end

function M.recycleComponent(comp)
    local cname = comp.__cname
    local Type = M.getComponentTypeByName(cname)
    local pool = getPool(Type) 
    if pool then pool:release(comp) end
end

function M.getOrCreateEntity()
    local entityPool = getEntityPool()
    local entity = entityPool.getOrCreate()
    entity._id = getNewEntityId()

    _entityIdDict[entity._id] = entity
    
    return entity
end

function M:getEntityById(entityId)
    return _entityIdDict[entityId]
end

function M.recycleEntity(entity)
    local entityId = entity._id
    local entityPool = getEntityPool()

    entity._id = 0
    entityPool:release(entity)
    _entityIdDict[entityId] = nil
    

end

--
function M.addWorld(world)
    table.insert(_worlds, world)
end
---
function M.getComponentTypeByName(cname)
    return _compCName2TypeDict[cname]
end

function M.addComponentTypeByType(Type)
    local cname = Type.cname
    if not M.getComponentTypeByName(cname) then
        _compCName2TypeDict[cname] = Type
    end
end

--
function M.update(dt)
    for _,world in pairs(_worlds) do 
        world:update(dt)
    end
end

----
function M.clear()

end

rawset(_G, "ECSManager", M)