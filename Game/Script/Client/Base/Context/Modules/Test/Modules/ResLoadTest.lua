local M = class("ResLoadTest")

function M:ctor( ... )
    ResourceLoader:loadEffect("60100002",function ( obj )
        print(15,obj)
    end)
end

return M