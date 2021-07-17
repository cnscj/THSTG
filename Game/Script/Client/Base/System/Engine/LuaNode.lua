local M = class("LuaNode")

function M:ctor()
    self._gameObject = false
end

function M:getGameObject( ... )
    
end

function M:getBehaviour( ... )
    
end

function M:addBehaviour( ... )

end

function M:getBehaviours( ... )

end

rawset(_G, "LuaNode", M)