---@class LuaBehaviour

local _StaticFuncs_ = {
    addBehaviour = function (N,ugo)
        local luaIns = N.new()
        local unityBehavior = ugo:AddComponent(typeof(CSharp.LuaBehaviour))
        unityBehavior:SetTable(luaIns)
        return luaIns
    end,
    
    getBehaviour = function (N,ugo)
        local unityBehaviors = ugo:GetComponents(typeof(CSharp.LuaBehaviour))
        for i = 0 ,unityBehaviors.Length - 1 do 
            local unityBehavior = unityBehaviors[i]
            if unityBehavior.LuaInstance.__cname == N.cname then
                return unityBehavior.LuaInstance
            end
        end
    end,
    
    getBehaviours = function(N,ugo)
        local unityBehaviors = ugo:GetComponents(typeof(CSharp.LuaBehaviour))
        if unityBehaviors and unityBehaviors.Length > 0 then
            local behaviours = {}
            for i = 0 ,unityBehaviors.Length - 1 do 
                local unityBehavior = unityBehaviors[i]
                if unityBehavior.LuaInstance.__cname == N.cname then
                    table.insert( behaviours,unityBehavior.LuaInstance )
                end
            end
            return behaviours
        end
        return {}
    end,
    
    destroyBehaviours = function(N,ugo)
        local unityBehaviors = ugo:GetComponents(typeof(CSharp.LuaBehaviour))
        for i = 0 ,unityBehaviors.Length - 1 do 
            local unityBehavior = unityBehaviors[i]
            if unityBehavior.LuaInstance.__cname == N.cname then
                CSharp.Object.Destroy(unityBehavior)
            end
        end
    end,
}

local M = class("LuaBehaviour", false, _StaticFuncs_)
function M:ctor()
    self.owner = false
    self.gameObject = false
end

function M:newWith( ... )

end

function M:delWith( ... )

end

--下面函数由子类自行添加
-- function M:awake() end
-- function M:start()end
-- function M:update()end
-- function M:lateUpdate()end
-- function M:fixedUpdate()end
-- function M:onEnable()end
-- function M:onDestroy()end

function M:setEnabled(val)
    if self.owner then self.owner.enabled = val end
end

function M:isEnabled()
    if self.owner then return self.owner.enabled end
end

function M:destroySelf()
    if self._owner then
        CSharp.Object.Destroy(self._owner)
    end
end

rawset(_G, "LuaBehaviour", M)