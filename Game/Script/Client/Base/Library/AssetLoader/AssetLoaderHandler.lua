local M = class("AssetLoaderHandler")

function M:ctor()
    self.baseLoader = false
    self.path = false
    self.result = false
    self.timeout = 30

    self._callbacks = false
    self._children = false
    self._report = false --返回失败报告,加载失败的资源路径集合
    self._callCount = 0
    self._doneTick = 0
    self._lastTick = 0
end

function M:isTimeout()
    local curTick = millisecondNow()
    return curTick - self._lastTick >= (self.timeout * 1000)
end

function M:isCompleted()
    return self._doneTick > 0
end

function M:tick()
    self._lastTick = millisecondNow()
end

function M:finish()
    if (self:isCompleted()) then return end
    
    self:_onSelfCall(self)
end

--
function M:_onTryCall()
    local needCallCount = (self._children and self._children:size() or 0) + 1
    if self._callCount >= needCallCount then        
        self._doneTick = millisecondNow()    --会不会引发等待?
        self:callCallback()
    end
end

function M:_onChildCall(childHandler)
    local childResult = childHandler.result
    if childResult then childResult:retain() end 

    self:_onSelfCall(childHandler)
end

function M:_onSelfCall(handler)
    self._callCount = self._callCount + 1

    --收集结果,如果存在加载失败的,收集起来返回
    if handler then
        local result = handler.result 
        if not result.data then
            self._report = self._report or {}
            table.insert( self._report, handler.path )
        end
    end
    
    self:_onTryCall()
end
--

function M:callCallback()
    if self._callbacks then
        for _,v in pairs(self._callbacks) do 
            local func = v.func
            local caller = v.caller
            if func then func(caller,self) end
        end
    end
end

function M:addCallback(func, caller)
    self._callbacks = self._callbacks or {}
    table.insert(self._callbacks, { func = func, caller = caller })
end

function M:removeCallback(func, caller)
    if self._callbacks then
        for i,v in pairs(self._callbacks) do 
            if not caller then
                if v.func == func then
                    table.remove(i)
                end
            elseif v.func == func and v.caller == caller then
                table.remove(i)
            end
        end
    end
end
--

function M:addChild(childHandler)
    self._children = self._children or Set.new()
    if not self._children:contains(childHandler) then 
        childHandler:addCallback(self._onChildCall,self)
        self._children:insert(childHandler)
    end
end

function M:removeChild(childHandler)
    if self._children then
        if self._children:contains(childHandler) then 
            childHandler:removeCallback(self._onChildCall,self)
            self._children:remove(childHandler)
        end
    end
end
--


rawset(_G, "AssetLoaderHandler", M)
