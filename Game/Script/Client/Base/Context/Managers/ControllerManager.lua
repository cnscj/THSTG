local M = class("ControllerManager")
local _LIST = require("Config.Profile.P_Controller")
function M:ctor()
    self._allControllers = {}
end

function M:initialize()
    for _,info in ipairs(_LIST) do 
        local cls = require(info.path)
        if cls then
            local ins = cls.new()
            table.insert(self._allControllers, ins)
        else
            printWarning(string.format( "%s not find",info.path))
        end
    end
end

function M:clear( ... )

end


rawset(_G, "ControllerManager", false)
ControllerManager = M.new()