local M = class("ObjectPoolManager")

function M:ctor()
    self._poolDict = {}

    --注册一个轮询函数
    self._updateFunction = function ( ... )
        self:update(CSharp.Time.deltaTime)
    end
end

function M:init()
    --注册更新
    CSharp.MonoManagerIns:AddUpdateListener(self._updateFunction)
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

function M:getOrCreatePool(Type)
   return self:getPool(Type) or self:createPool(Type)
end

function M:clearAll()
    for _,pool in pairs(self._poolDict) do 
        pool:clearAll()
    end
    self._poolDict = {}
end

function M:update(dt)
    -- 自动清理的轮询
    for _,pool in pairs(self._poolDict) do 
        pool:update(dt)
    end
end

rawset(_G, "ObjectPoolManager", false)
ObjectPoolManager = M.new()