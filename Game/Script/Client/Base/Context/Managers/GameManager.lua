local M = class("GameManager")

function M:ctor()

end

function M:start()

end

function M:update(dt)
 
end

function M:reset()

end

function M:clear()

end

---

function M:registerUpdateHandler(handler)

end

function M:unregisterUpdateHandler(key)
    
end

rawset(_G, "GameManager", false)
GameManager = M.new()