
local M = class("TestSystem2",ECS.System)
local TestComponent = require("Context.Modules.Test.Modules.ECSTest.TestComponent")
local TestComponent2 = require("Context.Modules.Test.Modules.ECSTest.TestComponent2")

function M:ctor()
    self._listenedComponents = {TestComponent2}
end

function M:modifyUpdate(entities)
    for _,entity in pairs(entities) do 
        local testComp2 = entity:getComponent(TestComponent2)

        print(15,testComp2.test)
    end

end

return M