
local M = class("ECSTest")

function M:ctor()
    local TestComponent = require("Context.Modules.Test.Modules.ECSTest.TestComponent")
    local TestComponent2 = require("Context.Modules.Test.Modules.ECSTest.TestComponent2")
    ECSManager:registerComponentClass(TestComponent)
    ECSManager:registerComponentClass(TestComponent2)
    -- print(15,TestComponent.cname)

    local TestSystem = require("Context.Modules.Test.Modules.ECSTest.TestSystem")
    local TestSystem2 = require("Context.Modules.Test.Modules.ECSTest.TestSystem2")

    local myWorld = ComponentSystemWorld:getGameWorld()
    myWorld:addSystem(TestSystem.new())
    myWorld:addSystem(TestSystem2.new())

    local myEntity = ECSManager:createEntity()
    myEntity:addComponent(TestComponent)
    myEntity:addComponent(TestComponent2)

    local myEntity2 = ECSManager:createEntity()
    myEntity2:addComponent(TestComponent)
    myEntity2:addComponent(TestComponent2)

    local myEntity3 = ECSManager:createEntity()
    myEntity3:addComponent(TestComponent)

    myEntity:addToWorld(myWorld)
    myEntity2:addToWorld(myWorld)
    myEntity3:addToWorld(myWorld)
    Timer:scheduleOnce(3,function ( ... )
        local testComp = myEntity2:getComponent(TestComponent)
        testComp.data1 = "!11"
        myEntity2:replaceComponent(testComp)
    end)
end

return M