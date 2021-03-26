local M = {}
function M:ctor()
    self._componentPoolMgr = ObjectPoolManager.new()        --对象池管理器
    self._groupPool = false                                 --组件对象池
    self._matcherPool = false                               --组件对象池
end

function M:createComponent(Type)
    local pool = self._componentPoolMgr:getPool(Type) or self._componentPoolMgr:createPool(Type)
    local comp = pool:getOrCreate(type)
    return comp
end


function M:recycleComponent(comp)
    --key = component.__cname
    local Type = false   --TODO:
    local pool = self._componentPoolMgr:getPool(Type) 
    if pool then
        pool:release(comp)
    end
end

function M:createGroup()

end


function M.clearAll()

end


