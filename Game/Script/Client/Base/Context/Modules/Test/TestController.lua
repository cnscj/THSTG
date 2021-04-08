local M = class("TestController",Controller)
local RedDotTest = require("Context.Modules.Test.Modules.RedDotTest")

function M:ctor()
    Timer:scheduleOnce(10,function ( ... )
        self:dispatchEvent(EventType.TEST_1)
    end)

    self.redDotTest = RedDotTest.new()
end

function M:_initListeners()
    self:addEventListener(EventType.TEST_1, self._print, self)
end

function M:_print(e,params)
    local val = Cache.testCache:getTestVal()
    print(15,val)
end

return M