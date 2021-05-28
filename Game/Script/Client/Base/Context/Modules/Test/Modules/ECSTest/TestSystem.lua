
local M = class("TestSystem",ECS.System)
local TestComponent = require("Context.Modules.Test.Modules.ECSTest.TestComponent")
local TestComponent2 = require("Context.Modules.Test.Modules.ECSTest.TestComponent2")

function M:ctor()
    self._listenedComponents = {TestComponent}
end

function M:modifyUpdate(entities)
    for _,entity in pairs(entities) do 
        local testComp = entity:getComponent(TestComponent)

        -- print(15,"!!!!!")
        print(15,testComp.data1)
    end

end

return M