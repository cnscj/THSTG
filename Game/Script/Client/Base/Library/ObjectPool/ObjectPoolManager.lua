local M = class("ObjectPoolManager")

function M:ctor()
    self._dict = {}
end

function M:createPool(Type)
    if not self._dict[Type] then
        self._dict[Type] = ObjectPool.new(Type)
    end
    return self._dict[Type]
end

function M:getPool(Type)
    return self._dict[Type]
end

function M:clearAll()
    for _,pool in pairs(self._dict) do 
        pool:clearAll()
    end
    self._dict = {}
end