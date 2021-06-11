local M = class("TestController",MVC.Controller)
local RedDotTest = require("Context.Modules.Test.Modules.RedDotTest")
local CoroutineTest = require("Context.Modules.Test.Modules.CoroutineTest")
local ResLoadTest = require("Context.Modules.Test.Modules.ResLoadTest")
local ECSTest = require("Context.Modules.Test.Modules.ECSTest.ECSTest")
local LuaBehaviourTest = require("Context.Modules.Test.Modules.LuaBehaviourTest")
function M:ctor()
    Timer:scheduleOnce(10,function ( ... )
        self:dispatchEvent(EventType.TEST_1)
    end)

    -- self.redDotTest = RedDotTest.new()
    -- self.resLoadTest = ResLoadTest.new()
    self.ecsTest = ECSTest.new()
    LuaBehaviourTest.new()
    
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