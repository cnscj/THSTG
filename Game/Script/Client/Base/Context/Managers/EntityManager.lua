local M = class("EntityManager")

function M:ctor()
    self._hero = false
end

function M:createHero()
    if not self._hero then 
        self._hero = ECSManager:createEntity()

    end
    return self._hero
end

function M:getHero()
    return self._hero
end


function M:clear()
    
end

rawset(_G, "EntityManager", false)
EntityManager = M.new()