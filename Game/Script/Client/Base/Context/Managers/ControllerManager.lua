local M = class("ControllerManager")
local _LIST = require("Config.Profile.P_Controller")
function M:ctor()

end

function M:initialize()
    for _,info in ipairs(_LIST) do 
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
        MVCManager:removeController(classPath)

        local ins = cls.new()
        MVCManager:removeController(classPath,ins)
    end
end

function M:clear()
    
end

rawset(_G, "ControllerManager", false)
ControllerManager = M.new()