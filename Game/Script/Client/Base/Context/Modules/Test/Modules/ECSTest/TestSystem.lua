
local M = class("TestSystem",ECS.System)
local TestComponent = require("Context.Modules.Test.Modules.ECSTest.TestComponent")
local TestComponent2 = require("Context.Modules.Test.Modules.ECSTest.TestComponent2")

function M:ctor()
    
end

local totalDt = 0
function M:update(dt)
    totalDt = totalDt + dt
    if totalDt >= 5 then
        local archetype = ECSManager:getArchetypeByComponentsClass(TestComponent,TestComponent2)
        local entities = self:getEntities(archetype)
        dump(15,entities)
        -- local entity = self:getEntities()


        totalDt = 0
    end
end

return M