local M = class("CacheManager")
local GLOBAL_PACKAGE = "Cache"

local _LIST = require("Config.Profile.P_Cache")

function M:ctor()
    self._cacheDict = {}
end

function M:initialize()
    for _,info in ipairs(_LIST) do 
        local cls = require(info.path)
        if cls then
            local ins = cls.new()
            self._cacheDict[info.name] = ins
        else
            printWarning(string.format( "%s not find",info.path))
        end
    end
    rawset(_G, GLOBAL_PACKAGE, self._cacheDict)
end

function M:clear()

end

rawset(_G, "CacheManager", false)
CacheManager = M.new()