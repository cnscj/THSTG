local GLOBAL_CACHE_PACKAGE = "Cache"

local M = class("ControllerManager")
local P_MVC = require("Config.Profile.P_MVC")
local CACHE_LIST = P_MVC.caches
local CONTROLLER_LIST = P_MVC.controllers

function M:ctor()
    self._cacheKeyDict = {}
end

function M:initialize()
    self:initializeCache()
    self:initializeController()
end


function M:initializeCache()
    for _,info in ipairs(CACHE_LIST) do 
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
    rawset(_G, GLOBAL_CACHE_PACKAGE, cacheDict)
end


function M:initializeController( ... )
    for _,info in ipairs(CONTROLLER_LIST) do 
        local classPath = info.path
        local cls = require(classPath)
        if cls then
            local ins = cls.new()
            MVCManager:addController(classPath,ins)
        else
            printWarning(string.format("%s not find",classPath))
        end
    end
end

-- 暂时只是用来刷reload
function M.reloadController(classPath, newClass)
    local cls = MVCManager:getController(classPath)

    if cls then
        local ins = cls.new()
        MVCManager:replaceController(classPath,ins)
    end
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
    
    MVCManager:replaceCache(key,newCache)
end

function M:clear()
    
end

rawset(_G, "CacheControllerManager", false)
CacheControllerManager = M.new()