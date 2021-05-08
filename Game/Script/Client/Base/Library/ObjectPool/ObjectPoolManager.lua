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

function M:createPool(cls)
    if not cls then return end 
    if not cls.new then return end --是个类而不是实例

    if not self._poolDict[cls] then
        self._poolDict[cls] = ObjectPool.new(cls)
    end
    return self._poolDict[cls]
end

function M:getPool(cls)
    return self._poolDict[cls]
end

function M:addPool(pool)
    if not pool then return end
    local cls = pool._cls

    self._poolDict[cls] = pool
end

function M:getOrCreatePool(cls)
   return self:getPool(cls) or self:createPool(cls)
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