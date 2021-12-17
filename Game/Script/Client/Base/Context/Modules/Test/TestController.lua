local M = class("TestController",MVC.Controller)
-- local RedDotTest = require("Context.Modules.Test.Modules.RedDotTest")
-- local CoroutineTest = require("Context.Modules.Test.Modules.CoroutineTest")
local FGUITest = require("Context.Modules.Test.Modules.FGUITest")
-- local ResLoadTest = require("Context.Modules.Test.Modules.ResLoadTest")
-- local ECSTest = require("Context.Modules.Test.Modules.ECSTest.ECSTest")
-- local LuaBehaviourTest = require("Context.Modules.Test.Modules.LuaBehaviourTest")
local BinaryTest = require("Context.Modules.Test.Modules.BinaryTest")

local HTTPTest = require("Context.Modules.Test.Modules.HTTPTest")

function M:ctor()
    Timer:scheduleOnce(10,function ( ... )
        self:dispatchEvent(EventType.TEST_1)
    end)

    -- self.redDotTest = RedDotTest.new()
    -- self.resLoadTest = ResLoadTest.new()

    -- self.ecsTest = ECSTest.new()
    -- LuaBehaviourTest.new()

    -- self.coroutineTest = CoroutineTest.new()
    -- self.fguiTest = FGUITest.new()
    self.httpTest = HTTPTest.new()
    -- self.binaryTest = BinaryTest.new()
    
    print(15,"@@@@@@@@@@@@")
end

function M:_initListeners()
    self:addEventListener(EventType.TEST_1, self._print)
end

function M:_print(e,params)
    local val = Cache.testCache:getTestVal()
    print(15,val)
end

return M