local M = class("TestComponent",ECS.Component)

function M:ctor()
    self.data1 = false    
end

return M