--[[
    NOTE: 关于Event被移除问题
    如果在多个界面有这个组件,当其中一个被close,移除消息之后,其他界面存在这个组件的都无法接收消息
    解决方法:
    在ctor里
    self.__updateLayer = function()
        self:_updateLayer()
    end
    然后注册这么注册
    self:addEventListener(EventType.ACTIVITY_MSG_UPDATE, self.__updateLayer)
    参考SkillPanel.lua
]]
---@class FComponent:GComponent
local M = class("FComponent", GComponent)

function M:ctor(obj)
    self._obj = obj
    self._root = false          --这是为了习惯统一,这是为了习惯统一,这是为了习惯统一
    self._children = {}
    self._eventListeners = {}
    self._timerInterval = false
    self._timerId = false
    self._viewName = self.__cname

    -- 子默认不开
    self._fairyBatching = false
end

function M:init(obj)
    self._root = self
    if self._fairyBatching then
        self._obj.fairyBatching = self._fairyBatching
    end
    if self._obj then
        self._obj.onAddedToStage:Add(function ()
            self:__onEnter()
        end)
        self._obj.onRemovedFromStage:Add(function ()
            self:__onExit()
        end)
        self:_initUI()
    end
end




-- 显示的时候触发
function M:__onEnter()
    self:_initEvent()
    self:_enter()
    self:__runTimer()
end

-- 隐藏的时候触发
function M:__onExit()
    self:_exit()
    self:__clearEventListeners()
    self:__clearTimer()
end

-- 删除所有侦听的事件
function M:__clearEventListeners()
    for _, event in ipairs(self._eventListeners) do
        Dispatcher.removeEventListener(event.name, event.listener, self)
    end
end

-- 运行定时器
function M:__runTimer()
    if type(self._timerInterval) == "number" 
        and self._timerInterval >= 0 
        and self._timerId == false
    then
        self._timerId = Timer:schedule(function ()
            self:_onTick()
        end, self._timerInterval, 0)
        self:_onTick()
    end
end

-- 清除定时器
function M:__clearTimer()
    if self._timerId then
        Timer:unschedule(self._timerId)
        self._timerId = false
    end
end

--------------------------------------------------

-- 重写方法
function M:_initUI()

end

function M:_initEvent()

end

function M:_enter()

end

function M:_exit()

end

function M:_onTick()

end

--------------------------------------------------

-- 公有方法
function M:addEventListener(name, listener, listenerCaller, priority)
    Dispatcher.addEventListener(name, listener, listenerCaller, priority)
    table.insert(self._eventListeners, {name=name, listener=listener})
end

function M:addListener(name, listener, priority)
    Dispatcher.addEventListener(name, listener, self, priority)
    table.insert(self._eventListeners, { name = name, listener = listener })
end

rawset(_G, "FComponent", M)
