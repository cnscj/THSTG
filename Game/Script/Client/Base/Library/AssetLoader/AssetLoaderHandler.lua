local M = class("AssetLoaderHandler",false,{
   
})

function M:ctor()
    self.baseLoader = false
    self.path = false
    self.result = false

    self._callbacks = {}
    self._children = false
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
    for _,v in pairs(self._callbacks) do 
        local func = v.func
        local caller = v.caller
        if func then func(caller,self.result) end
    end
    self._isCompleted = true
end
--
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
function M:addChild(handler)
    self._children = self._children or {}
    if not self._children[handler] then 
        self._children[handler] = handler
    end
end

function M:getChildren()
    return self._children
end
--


rawset(_G, "AssetLoaderHandler", M)
