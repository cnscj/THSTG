local M = class("MonoManager")
local MonoManagerIns = CSharp.MonoManagerIns
function M:ctor()
    self.awakeListeners = false
    self.startListeners = false
    self.updateListeners = false
    self.fixedUpdateListeners = false
    self.lateUpdateListeners = false

    self._awakeFunc = function ( ... ) self:_awake() end
    self._startFunc = function ( ... ) self:_start() end
    self._updateFunc = function ( ... ) self:_update() end
    self._fixedUpdateFunc = function ( ... ) self:_fixedUpdate() end
    self._lateUpdateFunc = function ( ... ) self:_lateUpdate() end
end

function M:addAwakeListener(listener,caller)
    if not self.awakeListeners then
        self.awakeListeners = {}
        MonoManagerIns:AddUpdateListener(self._awakeFunc)
    end
    table.insert(self.awakeListeners, {listener = listener,caller = caller})
end

function M:removeAwakeListener(listener,caller)
    for i = #self.awakeListeners,1,-1 do 
        local v = self.awakeListeners[i]
        if v.listener == listener and v.caller == caller then
            table.remove(self.awakeListeners,i)
            break
        end
    end
end

function M:addStartListener(listener,caller)
    if not self.startListeners then
        self.startListeners = {}
        MonoManagerIns:AddUpdateListener(self._startFunc)
    end
    table.insert(self.startListeners, {listener = listener,caller = caller})
end

function M:removeStartListener(listener,caller)
    for i = #self.startListeners,1,-1 do 
        local v = self.startListeners[i]
        if v.listener == listener and v.caller == caller then
            table.remove(self.startListeners,i)
            break
        end
    end
end

function M:addUpdateListener(listener,caller)
    if not self.updateListeners then
        self.updateListeners = {}
        MonoManagerIns:AddUpdateListener(self._updateFunc)
    end
    table.insert(self.updateListeners, {listener = listener,caller = caller})
end

function M:removeUpdateListener(listener,caller)
    for i = #self.updateListeners,1,-1 do 
        local v = self.updateListeners[i]
        if v.listener == listener and v.caller == caller then
            table.remove(self.updateListeners,i)
            break
        end
    end
end

function M:addFixedUpdateListener(listener,caller)
    if not self.fixedUpdateListeners then
        self.fixedUpdateListeners = {}
        MonoManagerIns:AddFixUpdateListener(self._fixedUpdateFunc)
    end
    table.insert(self.fixedUpdateListeners, {listener = listener,caller = caller})
end

function M:removeFixedUpdateListener(listener,caller)
    for i = #self.fixedUpdateListeners,1,-1 do 
        local v = self.fixedUpdateListeners[i]
        if v.listener == listener and v.caller == caller then
            table.remove(self.fixedUpdateListeners,i)
            break
        end
    end
end

function M:addLateUpdateListener(listener,caller)
    if not self.lateUpdateListeners then
        self.lateUpdateListeners = {}
        MonoManagerIns:AddFixUpdateListener(self._lateUpdateFunc)
    end
    table.insert(self.lateUpdateListeners, {listener = listener,caller = caller})
end

function M:removeLateUpdateListener(listener,caller)
    for i = #self.lateUpdateListeners,1,-1 do 
        local v = self.lateUpdateListeners[i]
        if v.listener == listener and v.caller == caller then
            table.remove(self.lateUpdateListeners,i)
            break
        end
    end
end

function M:removeAllListeners()
    self.updateListeners = {}
    self.fixedUpdateListeners = {}
    self.lateUpdateListeners = {}
end
--
function M:_awake()
    if self.awakeListeners then
        for _,funcArgs in ipairs(self.awakeListeners) do 
            funcArgs.listener(funcArgs.caller)
        end
    end
end

function M:_start()
    if self.startListeners then
        for _,funcArgs in ipairs(self.startListeners) do 
            funcArgs.listener(funcArgs.caller)
        end
    end
end

function M:_update()
    if self.updateListeners then
        local deltaTime = CSharp.Time.deltaTime
        for _,funcArgs in ipairs(self.updateListeners) do 
            funcArgs.listener(funcArgs.caller,deltaTime)
        end
    end
end

function M:_fixedUpdate()
    if self.fixedUpdateListeners then
        local deltaTime = CSharp.Time.deltaTime
        for _,funcArgs in ipairs(self.fixedUpdateListeners) do 
            funcArgs.listener(funcArgs.caller,deltaTime)
        end
    end
end

function M:_lateUpdate()
    if self.lateUpdateListeners then
        local deltaTime = CSharp.Time.deltaTime
        for _,funcArgs in ipairs(self.lateUpdateListeners) do 
            funcArgs.listener(funcArgs.call,deltaTime)
        end
    end
end

rawset(_G, "MonoManager", false)
MonoManager = M.new()