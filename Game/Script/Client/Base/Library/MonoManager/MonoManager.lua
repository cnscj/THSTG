local M = class("MonoManager")
local MonoManagerIns = CSharp.MonoManagerIns
function M:ctor()
    self.updateFunc = false
    self.fixUpdateFunc = false
end

function M:addUpdateListener(listener,caller)
    if not self.updateFunc then
        self.updateFunc = {}
        MonoManagerIns:AddUpdateListener(self.update)
    end
    table.insert(self.updateFunc, {listener = listener,caller = caller})
end

function M:removeUpdateListener(listener,caller)
    for i = #self.updateFunc,1,-1 do 
        local v = self.updateFunc[i]
        if v.listener == listener and v.caller == caller then   --防止移除到基类listener
            table.remove(self.updateFunc,i)
            break
        end
    end
end

--
function M:update()
    local deltaTime = CSharp.Time.deltaTime
    for _,funcArgs in ipairs(self.updateFunc) do 
        funcArgs.listener(deltaTime)
    end
end
rawset(_G, "MonoManager", false)
MonoManager = M.new()