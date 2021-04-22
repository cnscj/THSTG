local M = class("MVCManager")

function M:ctor()
    self._ctrlDict = {}
    self._cacheDict = {}
end

function M:addController(key,ctrl)
    if not ctrl then
        return false
    end
    if not key then
        return false
    end

    if not self._ctrlDict[key] then
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

function M:getCache(key)
    return self._cacheDict[key]
end

function M:getCacheDict()
    return self._cacheDict
end



rawset(_G, "MVCManager", M)
MVCManager = M.new()