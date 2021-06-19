local M = class("TestComponent2",ECS.Component)

function M:ctor( ... )
    self.str = ""
end

return M