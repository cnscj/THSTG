local M = class("RedDotTest")

function M:ctor( ... )
    RedDotManager:register(function ( )
        print(15,"@@@@@")
    end,"tt","aa","ff")
    RedDotManager:register(function ( )
        print(15,"####")
    end,"tt","aa")

    RedDotManager:unregister("tt","aa","ff")
    -- RedDotManager:update("tt","aa","ff")
    dump(0,RedDotManager._root)
end

return M