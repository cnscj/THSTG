---@class Controller
local M = class("Controller")

function M:ctor()
    self.__eventListeners = {}
end

function M:init()
    self:_initListeners()
end

-- 初始化Listener时调用，待子类重写
function M:_initListeners()
end

-- 事件侦听方法，主要用于侦听服务端下推
function M:addListener(name, listener, priority)
    Dispatcher:addEventListener(name, listener, self, priority)
    self:__pushListener(name, listener)
end

function M:removeListener(name, listener)
    Dispatcher:removeEventListener(name, listener, self)
    self:__popListener(name, listener)
end

function M:addEventListener(name, listener, listenerCaller, priority)
    Dispatcher:addEventListener(name, listener, listenerCaller, priority)
    self:__pushListener(name, listener)
end

function M:removeEventListener(name, listener)
    Dispatcher:removeEventListener(name, listener, self)
    self:__popListener(name, listener)
end

function M:dispatchEvent(name,...)
    return Dispatcher:dispatchEvent(name,...)
end

-- 
function M:__pushListener(name, listener)
    local t = self.__eventListeners[name]
    if not t then
        t = {}
        self.__eventListeners[name] = t
    end
    t[listener] = true
end

function M:__popListener(name, listener)
    local t = self.__eventListeners[name]
    if t then
        t[listener] = nil
    end
end

function M:reloadClear()
    for k1, v1 in pairs(self.__eventListeners) do
        for k2, v2 in pairs(v1) do
            Dispatcher:removeEventListener(k1, k2, self)
        end
        self.__eventListeners[k1] = nil
    end
end


rawset(_G, "Controller", false)
Controller = M