local M = class("ConfigManager")
local GLOBAL_PACKAGE = "Config"
local _LIST = require("Config.Profile.P_Config")
--XXX:配置文件可能会优先与Game启动,不应该受Manager管理,比如资源配置,语言配置
function M:ctor()
    self._configsDict = {}
end

function M:initialize()
    for _,info in ipairs(_LIST) do 
        local cls = require(info.path)
        if cls then
            local ins = cls.new()
            self._configsDict[info.name] = ins
        else
            printWarning(string.format( "%s not find",info.path))
        end
    end
    rawset(_G, GLOBAL_PACKAGE, self._configsDict)
end

function M:clear( ... )

end

rawset(_G, "ConfigManager", false)
ConfigManager = M.new()