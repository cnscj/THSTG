local M = {}

local _poolDict = {}

function M.createPool(Type)
    if not Type then return end 
    if not Type.new then return end --是个类而不是实例

    if not _poolDict[Type] then
        _poolDict[Type] = ObjectPool.new(Type)
    end
    return _poolDict[Type]
end

function M.getPool(Type)
    return _poolDict[Type]
end

function M.clearAll()
    for _,pool in pairs(_poolDict) do 
        pool:clearAll()
    end
    _poolDict = {}
end

rawset(_G, "ObjectPoolManager", M)