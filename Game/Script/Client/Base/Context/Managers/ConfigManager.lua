local M = class("ConfigManager")
local _LIST = require("Config.Profile.P_Config")
function M:ctor()
    self._configsDict = {}
end

function M:init()
    for _,info in ipairs(_LIST) do 
        local cls = require(info.path)
        if cls then
            local ins = cls.new()
            self._configsDict[info.name] = ins
        end
    end
    rawset(_G, "Config", self._configsDict)
end

function M:clear( ... )

end