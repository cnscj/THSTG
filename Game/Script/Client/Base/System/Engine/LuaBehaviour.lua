---@class LuaBehaviour
local M = class("LuaBehaviour")
function M.addToGameObject(ugo)
    if not ugo then return end
    local newLuaComponent = M.new(ugo)
    local newUnityComponent = ugo:AddComponent(CSharp.LuaBehaviour)
    newUnityComponent:set(newLuaComponent)--TODO:

    return newLuaComponent
end

function M.destroyBehaviour(behaviour)
    local owner = behaviour.gameObject
    local allUnityComponents = behaviour:GetComponents(CSharp.LuaBehaviour)
    for i = 0,#allUnityComponents do 
        local comp = allUnityComponents[i]
        if comp.lua == behaviour then
            CSharp.Object.Destroy(comp)
            break
        end
    end
end
---
function M:ctor(owner)
    self.gameObject = owner
end

--

function M:awake()

end

function M:start()

end

function M:update()

end

function M:lateUpdate()

end

function M:fixedUpdate()

end

function M:onEnabled()

end

function M:onDestroy()

end

return M