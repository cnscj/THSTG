--[[
    缓存Component各种数据
]]
--TODO:生命周期不可控
local M = class("UnityGameObject",false,{
    initWithGameObject = false,
    initWithNew,
})
function M:ctor(gameObject)
    self._obj = gameObject
end

function M:getObj()
    return self._obj
end

function M:getComponent( ... )

end

function M:getComponents( ... )

end

function M:addComponent( ... )

end

M.initWithNew = function (name,parent)
    local ins = M.new()
    local go = GameObject()
    if not string.isEmpty(name) then
        go.name = name
    end

    if parent then
        go.transform.setParent(parent)
    end
    ins._obj = go
end

M.initWithGameObject = function ( go )
    local ins = M.new()
    ins._obj = go

    return ins
end

rawset(_G, "UnityGameObject", M)