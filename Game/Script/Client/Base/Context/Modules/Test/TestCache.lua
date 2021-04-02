local M = class("TestCache",BaseCache)

function M:ctor()
    self._testVal = 1
end

function M:setTestVal(val)
    self._testVal = val or false
end

function M:getTestVal()
    return self._testVal
end

return M