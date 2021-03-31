local M = class("EntityManager")
function M:ctor()
    self._hero = false
end

function M:createHero()
    
end

function M:getHero( ... )

end

rawset(_G, "EntityManager", false)
EntityManager = M.new()