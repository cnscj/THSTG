local M = class("MonoManager")
local MonoManagerIns = CSharp.MonoManagerIns
function M:ctor()
    self.awakeFunc = false
    self.startFunc = false
    self.updateFunc = false
    self.fixedUpdateFunc = false
    self.lateUpdateFunc = false
end

function M:addAwakeListener(listener,caller)
    if not self.awakeFunc then
        self.awakeFunc = {}
        MonoManagerIns:AddUpdateListener(self._awake)
    end
    table.insert(self.awakeFunc, {listener = listener,caller = caller})
end

function M:removeAwakeListener(listener,caller)
    for i = #self.awakeFunc,1,-1 do 
        local v = self.awakeFunc[i]
        if v.listener == listener and v.caller == caller then
            table.remove(self.awakeFunc,i)
            break
        end
    end
end

function M:addStartListener(listener,caller)
    if not self.startFunc then
        self.startFunc = {}
        MonoManagerIns:AddUpdateListener(self._start)
    end
    table.insert(self.startFunc, {listener = listener,caller = caller})
end

function M:removeStartListener(listener,caller)
    for i = #self.startFunc,1,-1 do 
        local v = self.startFunc[i]
        if v.listener == listener and v.caller == caller then
            table.remove(self.startFunc,i)
            break
        end
    end
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
        if v.listener == listener and v.caller == caller then
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
        if v.listener == listener and v.caller == caller then
            table.remove(self.fixedUpdateFunc,i)
            break
        end
    end
end

function M:addLateUpdateListener(listener,caller)
    if not self.lateUpdateFunc then
        self.lateUpdateFunc = {}
        MonoManagerIns:AddFixUpdateListener(self._lateUpdate)
    end
    table.insert(self.lateUpdateFunc, {listener = listener,caller = caller})
end

function M:removeLateUpdateListener(listener,caller)
    for i = #self.lateUpdateFunc,1,-1 do 
        local v = self.lateUpdateFunc[i]
        if v.listener == listener and v.caller == caller then
            table.remove(self.lateUpdateFunc,i)
            break
        end
    end
end

function M:removeAllListeners()
    self.updateFunc = {}
    self.fixedUpdateFunc = {}
    self.lateUpdateFunc = {}
end
--
function M:_awake()
    if self.awakeFunc then
        for _,funcArgs in ipairs(self.awakeFunc) do 
            funcArgs.listener()
        end
    end
end

function M:_start()
    if self.startFunc then
        for _,funcArgs in ipairs(self.startFunc) do 
            funcArgs.listener()
        end
    end
end

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

function M:_lateUpdate()
    if self.lateUpdateFunc then
        local deltaTime = CSharp.Time.deltaTime
        for _,funcArgs in ipairs(self.lateUpdateFunc) do 
            funcArgs.listener(deltaTime)
        end
    end
end

rawset(_G, "MonoManager", false)
MonoManager = M.new()