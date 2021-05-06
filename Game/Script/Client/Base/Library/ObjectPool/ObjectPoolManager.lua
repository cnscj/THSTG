local M = class("ObjectPoolManager")

function M:ctor()
    self._poolDict = {}

    --注册一个轮询函数
    self._updateFunction = function ( ... )
        self:update(CSharp.Time.deltaTime)
    end

    --注册更新
    CSharp.MonoManagerIns:AddUpdateListener(self._updateFunction)
end

function M:createPool(Cls)
    if not Cls then return end 
    if not Cls.new then return end --是个类而不是实例

    if not self._poolDict[Cls] then
        self._poolDict[Cls] = ObjectPool.new(Cls)
    end
    return self._poolDict[Cls]
end

function M:getPool(Cls)
    return self._poolDict[Cls]
end

function M:addPool(pool)
    if not pool then return end
    local Cls = pool._cls

    self._poolDict[Cls] = pool
end

function M:getOrCreatePool(Cls)
   return self:getPool(Cls) or self:createPool(Cls)
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