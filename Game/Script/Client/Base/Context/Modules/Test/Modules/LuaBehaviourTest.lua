
local N = class("LuaBehaviourT1",LuaBehaviour)

function N:ctor()
    self.tt = 0
end

function N:start( ... )
    print(15,self.tt,"hello world")

end
function N:onDestroy( ... )

end
--
local K = class("LuaBehaviourT2",LuaBehaviour)
function K:ctor()
    self.xx = 0
end
function K:start( ... )
    print(15,self.xx,"the test 2")
end
------------------
local M = class("LuaBehaviourTest")
function M:ctor()
    local newGo = CSharp.GameObject()
    newGo.name = "TestGo"

    local t1 = N:getBehaviour(newGo) or N:addBehaviour(newGo)
    local t2 = N:getBehaviour(newGo) or N:addBehaviour(newGo)
    local t3 = K:addBehaviour(newGo)
    t1.tt = 1111
    t2.tt = 2222
    t3.xx = 3333

end

return M