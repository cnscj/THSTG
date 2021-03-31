local M = class("CacheManager")
local _LIST = require("Config.Profile.P_Cache")

function M:ctor()
    self._cacheDict = {}
end

function M:init()
    for _,info in ipairs(_LIST) do 
        local cls = require(info.path)
        if cls then
            local ins = cls.new()
            self._cacheDict[info.name] = ins
        end
    end
    rawset(_G, "Cache", self._cacheDict)
end

function M:clear()

end