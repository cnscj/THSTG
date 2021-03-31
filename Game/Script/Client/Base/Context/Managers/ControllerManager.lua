local M = class("ControllerManager")
local _LIST = require("Config.Profile.P_Controller")
function M:ctor()
    self._allControllers = {}
end

function M:init()
    for _,info in ipairs(_LIST) do 
        local cls = require(info.path)
        if cls then
            local ins = cls.new()
            table.insert(self._allControllers, ins)
        end
    end
end

function M:clear( ... )

end