local M = class("MonoManager")
local MonoManagerIns = CSharp.MonoManagerIns
function M:ctor()
    self.updateFunc = false
    self.fixedUpdateFunc = false
end

function M:addUpdateListener(listener,caller)
    if not self.updateFunc then
        self.updateFunc = {}
        MonoManagerIns:AddUpdateListener(self._update)
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

function M:addFixedUpdateListener(listener,caller)
    if not self.fixedUpdateFunc then
        self.fixedUpdateFunc = {}
        MonoManagerIns:AddFixUpdateListener(self._fixedUpdate)
    end
    table.insert(self.fixedUpdateFunc, {listener = listener,caller = caller})
end

function M:removeFixedUpdateListener(listener,caller)
    for i = #self.fixedUpdateFunc,1,-1 do 
        local v = self.fixedUpdateFunc[i]
        if v.listener == listener and v.caller == caller then   --防止移除到基类listener
            table.remove(self.fixedUpdateFunc,i)
            break
        end
    end
end

function M:removeAllListeners()
    self.updateFunc = {}
    self.fixedUpdateFunc = {}
end
--
function M:_update()
    if self.updateFunc then
        local deltaTime = CSharp.Time.deltaTime
        for _,funcArgs in ipairs(self.updateFunc) do 
            funcArgs.listener(deltaTime)
        end
    end
end

function M:_fixedUpdate()
    if self.fixedUpdateFunc then
        local deltaTime = CSharp.Time.deltaTime
        for _,funcArgs in ipairs(self.fixedUpdateFunc) do 
            funcArgs.listener(deltaTime)
        end
    end
end
rawset(_G, "MonoManager", false)
MonoManager = M.new()