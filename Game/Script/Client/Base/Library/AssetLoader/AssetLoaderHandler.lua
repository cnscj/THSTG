local M = class("AssetLoaderHandler",false,{
   
})

function M:ctor()
    self.baseLoader = false
    self.path = false
    self.result = false

    self._callbacks = {}
    self._children = false
    self._callCount = 0
    self._isCompleted = false
end

function M:isTimeout( ... )
    return false
end

function M:isCompleted( ... )
    return self._isCompleted
end

function M:tick()

end

function M:finish()
    if (self:isCompleted()) then return end
    
    self._callCount = self._callCount + 1
    self:onTryCall()
end
--
function M:onTryCall()
    local needCallCount = (self._children and self._children:size() or 0) + 1
    if self._callCount >= needCallCount then
        self:callCallback() 
        self._isCompleted = true    --会不会引发等待?
    end
end

function M:onChildCall(childHandler)
    self._callCount = self._callCount + 1
    self:onTryCall()
end
--

function M:callCallback()
    for _,v in pairs(self._callbacks) do 
        local func = v.func
        local caller = v.caller
        if func then func(caller,self.result) end
    end
end

function M:addCallback(func , caller)
    table.insert(self._callbacks, { func = func, caller = caller })
end

function M:removeCallback(func, caller)
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
--
function M:addChild(childHandler)
    self._children = self._children or Set.new()
    if not self._children:contains(childHandler) then 
        childHandler:addCallback(self.onChildCall,self)
        self._children:insert(childHandler)
    end
end

function M:removeChild(childHandler)
    if self._children then
        if self._children:contains(childHandler) then 
            childHandler:removeCallback(self.onChildCall,self)
            self._children:remove(childHandler)
        end
    end
end
--


rawset(_G, "AssetLoaderHandler", M)
