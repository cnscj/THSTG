local M = simpleClass("GameManager")

function M:ctor()
    self._updateListeners = false
end

--在Login界面之前,可以先加载一部分模块
function M:launch()
    ResourceLoader:initialize()
    ComponentSystemWorld:initialize()
    CacheControllerManager:initialize()
    UIManager:initialize()

end

--游戏开始
function M:start()

end

function M:update(dt)
    self:_updateListener(dt)
end

function M:reset()

end

function M:clear()
    self._updateListeners = false

    CacheControllerManager:clear()
    ComponentSystemWorld:clear()
    UIManager:clear()
end

---

function M:registerUpdateListener(listener)
    if listener then
        self._updateListeners = self._updateListeners or {}
        table.insert(self._updateListeners, listener)
    end
end

function M:unregisterUpdateListener(listener)
    if self._updateListeners then
        if listener then
            for i,handler in pairs(self._updateListeners) do 
                if handler == listener then
                    table.remove(self._updateListeners,i)
                    break
                end
            end
        end
    end
end

function M:_updateListener(dt)
    if self._updateListeners then
        for _,listener in pairs(self._updateListeners) do 
            listener(dt)
        end
    end
end

rawset(_G, "GameManager", false)
GameManager = M.new()