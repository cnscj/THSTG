local M = simpleClass("EntitySystemWorld")
local P_ECS = require("Config.Profile.P_ECS")
local SYSTEM_LIST = P_ECS.systems
local COMPONENT_LIST = P_ECS.components
function M:ctor()
    self._gameWorld = false
end

function M:initialize()
    self:initializeSystem()
    self:initializeComponent()

    self:initializeMainWorld()

end
--
function M:initializeSystem()
    --注册System
    local systemPaths = SYSTEM_LIST
    if systemPaths then
        for _,path in pairs(systemPaths) do 
            local cls = require(path)
            if cls then
                
            end
        end
    end
end
function M:initializeComponent()
    --注册Component
    local componentsPaths = COMPONENT_LIST
    if componentsPaths then
        for _,v in pairs(componentsPaths) do 
            local cls = require(path)
            if cls then
                ECSManager:registerComponentClass(cls)
            end
        end
    end
end

function M:initializeMainWorld()
    self._gameWorld = ECS.World.new()
    ECSManager:addWorld(self._gameWorld)
end
--

function M:getGameWorld()
    return self._gameWorld
end

function M:clear()
    ECSManager:removeWorld(self._gameWorld)
    self._gameWorld = false
end

rawset(_G, "ComponentSystemWorld", false)
ComponentSystemWorld = M.new()