
local M = class("ECSTest")

function M:ctor()
    local TestComponent = require("Context.Modules.Test.Modules.ECSTest.TestComponent")
    local TestComponent2 = require("Context.Modules.Test.Modules.ECSTest.TestComponent2")
    ECSManager:registerComponentClass(TestComponent)
    ECSManager:registerComponentClass(TestComponent2)
    -- print(15,TestComponent.cname)

    local TestSystem = require("Context.Modules.Test.Modules.ECSTest.TestSystem")

    local myWorld = ComponentSystemWorld:getGameWorld()
    local myTestSystem = TestSystem.new()

    myWorld:addSystem(myTestSystem)

    local myEntity = ECSManager:createEntity()
    myEntity:addComponent(TestComponent.cname)
    myEntity:addComponent(TestComponent2.cname)

    -- print(15,"@@@5,")
    dump(15,myEntity)
end

return M