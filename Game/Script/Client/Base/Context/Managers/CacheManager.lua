local M = class("CacheManager")
local GLOBAL_PACKAGE = "Cache"

local _LIST = require("Config.Profile.P_Cache")

function M:ctor()
    self._cacheKeyDict = {}
end

function M:initialize()
    for _,info in ipairs(_LIST) do 
        local classPath = info.path
        local cacheName = info.name
        local cls = require(classPath)
        if cls then
            local ins = cls.new()
            MVCManager:addCache(cacheName,ins)

            self._cacheKeyDict[classPath] = cacheName
        else
            printWarning(string.format( "%s not find",classPath))
        end
    end

    local cacheDict = MVCManager:getCacheDict()
    rawset(_G, GLOBAL_PACKAGE, cacheDict)
end

function M:reloadCache(classPath, newClass)
    local key = self._cacheKeyDict[classPath]
    local cls = MVCManager:getCache(key)
    if cls == nil then
        return
    end

    local newCache = newClass.new()
    for k, v in pairs(newCache) do
        if cls[k] ~= nil then
            newCache[k] = cls[k]    --新的Cache使用原Cache数据
        end
    end
    
    MVCManager:removeCache(key)
    MVCManager:addCache(key,newCache)
end


function M:clear()

end

rawset(_G, "CacheManager", false)
CacheManager = M.new()