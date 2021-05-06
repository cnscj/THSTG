local M = class("WorldManager")

function M:ctor()
    self._hero = false

    self._mainWorld = false
end


function M:initialize()

end

function M:clear()
    
end
--

function M:createHero()
    
end

function M:getHero( ... )

end

rawset(_G, "WorldManager", false)
WorldManager = M.new()