local M = class("MVCManager")

function M:ctor()
    self._ctrlDict = {}
    self._cacheDict = {}

    --注册一个轮询函数
    self._updateFunction = function ( ... )
        self:update(CSharp.Time.deltaTime)
    end

    --注册更新
    -- CSharp.MonoManagerIns:AddUpdateListener(self._updateFunction)
end

function M:addController(key,ctrl)
    if not ctrl then
        return false
    end
    if not key then
        return false
    end

    if not self._ctrlDict[key] then
        ctrl:initialize()
        self._ctrlDict[key] = ctrl
    end
    return true
end

function M:getController(key)
    return self._ctrlDict[key]
end

function M:removeController(key)
    local ctrl = self:getController(key)
    if ctrl then 
        ctrl:clear()
        self._ctrlDict[key] = nil
    end
end

function M:replaceController(key,ctrl)
    self:removeController(key)
    self:addController(key,ctrl)
end

function M:getControllerDict()
    return self._cacheDict
end

function M:addCache(key,cache)
    if not cache then
        return false
    end
    if not key then
        return false
    end

    if not self._cacheDict[key] then
        self._cacheDict[key] = cache 
    end
    return true
end

function M:removeCache(key)
    local cache = self:getCache(key)
    if cache then 
        cache:clear()
        self._cacheDict[key] = nil
    end
end

function M:replaceCache(key,cache)
    self:removeCache(key)
    self:addCache(key,cache)
end

function M:getCache(key)
    return self._cacheDict[key]
end

function M:getCacheDict()
    return self._cacheDict
end

function M:update(dt)

end


rawset(_G, "MVCManager", M)
MVCManager = M.new()