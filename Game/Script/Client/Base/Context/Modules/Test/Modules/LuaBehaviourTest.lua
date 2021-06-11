
local N = class("LuaBehaviourT1",LuaBehaviour)

function N:ctor()
    self.tt = 0
end

function N:start( ... )
    print(15,self.tt,"hello world")

end
function N:onDestroy( ... )

end


local M = class("LuaBehaviourTest")
function M:ctor()
    local newGo = CSharp.GameObject()
    newGo.name = "TestGo"

    local t1 = N:addBehaviour(newGo)
    local t2 = N:addBehaviour(newGo)

    t1.tt = 1111
    t2.tt = 2222

end

return M