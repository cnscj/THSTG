---@class View
local M = class("View")

function M:ctor()
    self.__eventListeners = {}
end

function M:init()
    self:_initListeners()
end

-- 初始化Listener时调用，待子类重写
function M:_initListeners()
    
end

function M:addEventListener(name, listener, listenerCaller, priority)
    Dispatcher.addEventListener(name, listener, listenerCaller, priority)
    table.insert(self.__eventListeners, { name = name, listener = listener })
end

function M:removeEventListener(name, listener)
    local t = {}
    for _, v in ipairs(self.__eventListeners) do
        if name == v.name and listener == v.listener then
            Dispatcher.removeEventListener(v.name, v.listener)
        else
            table.insert(t, { name = v.name, listener = v.listener })
        end
    end
    self.__eventListeners = t
end


function M:reloadClear()
    for _, v in ipairs(self.__eventListeners) do
        Dispatcher.removeEventListener(v.name, v.listener)
    end
    self.__eventListeners = {}
end

rawset(_G, "View", false)
View = M