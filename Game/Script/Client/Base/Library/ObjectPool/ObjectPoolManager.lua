local M = class("ObjectPoolManager")

function M:ctor()
    self._poolDict = {}
end

function M:createPool(Type)
    if not Type then return end 
    if not Type.new then return end --是个类而不是实例

    if not self._poolDict[Type] then
        self._poolDict[Type] = ObjectPool.new(Type)
    end
    return self._poolDict[Type]
end

function M:getPool(Type)
    return self._poolDict[Type]
end

function M:clearAll()
    for _,pool in pairs(self._poolDict) do 
        pool:clearAll()
    end
    self._poolDict = {}
end

rawset(_G, "ObjectPoolManager", false)
ObjectPoolManager = M.new()