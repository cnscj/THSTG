--[[
]]
---@class FWidget:GComponent
local M = class("FWidget", GComponent)

function M:ctor(obj,args)
    -- FGUI中的资源包名
    self._package = args and args.package or ""
    -- 资源包中的组件
    self._component = args and args.component or ""
    -- 子默认不开,如果播放动效发现层级不对需要开启
    self._fairyBatching = false

    ---ui根节点(Lua层的GComponent)
    self._root = false
    --显示对象根节点(Unity层的GameObject)
    self._rootGO = false
    --加载方式,如果为true为异步,false,为同步
    self._loadMethod = false
    --开启一个定时器
    self._timerInterval = false

    self.__eventListeners = false
    self.__timerId = false
    self.__isLoading = false
end
--------------------------------------------------
function M:init(obj,args)
    self._root = self
    self._rootGO = obj and obj.displayObject.gameObject or false

    self:_initObj()
end

-- 准备addChild到场景
function M:toAdd(parent)
    parent = parent or self._parent
    if parent then
        if not parent:isDisposed() and self._root and not self._root:isDisposed() then
            parent:addChild(self._root)
            return true
        end
    end
    return false
end

function M:toCreate(onSuccess,onFailed)
    -- 创建时，先判断一下父节点
    if self._parent and self._parent:isDisposed() then
        printWarning(string.format( "parent node has been disposed"))
        if onFailed then onFailed() end
        return
    end

    if string.isEmpty(self._package) or string.isEmpty(self._component) then
        printWarning(string.format( "package or component can't be empty"))
        if onFailed then onFailed() end
        return 
    end

    self.__isLoading = true
    UIManager:loadPackage(self._package ,self._loadMethod, function ( packageWrap )
        self.__isLoading = false
        self:__loadPackageCallback(packageWrap)
        if onSuccess then onSuccess() end
    end,onFailed)
end

-- 添加监听
function M:addEventListener(name, listener, listenerCaller, priority)
    self.__eventListeners = self.__eventListeners or {}
    Dispatcher.addEventListener(name, listener, listenerCaller, priority)
    table.insert(self.__eventListeners, {name=name, listener=listener})
end

function M:removeEventListener(name, listener, listenerCaller)
    if self.__eventListeners then
        for _, event in ipairs(self.__eventListeners) do
            if event.name == name and event.listener == listener and event.listenerCaller == listenerCaller then
                Dispatcher.removeEventListener(name, listener, listenerCaller)
            end
        end
    end
end


--------------------
function M:__onAddedToStage( ... )
    self:__onEnter()
end

function M:__onRemovedFromStage()
    self:__onExit()
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
    if self.__eventListeners then
        for _, event in ipairs(self.__eventListeners) do
            Dispatcher.removeEventListener(event.name, event.listener, self)
        end
    end
end

function M:__loadPackageCallback(packageWrap)
    self._obj = UIManager:createObject(self._package ,self._component)
    if not self._obj then
        printError(string.format("Check that component %s is set to export", self._component))
        return 
    end 

    self._root = self
    self._rootGO = self._obj and self._obj.displayObject.gameObject or false

    --FIXME:如果只是生成,没有AddChild,可能会运作不正常
    self._obj.onAddedToStage:Add(function ()
        packageWrap:retain()
    end)
    self._obj.onRemovedFromStage:Add(function ()
        packageWrap:release()
    end)

    self:_initObj()
end

-- 运行定时器
function M:__runTimer()
    if type(self._timerInterval) == "number" 
        and self._timerInterval >= 0 
        and self.__timerId == false
    then
        self.__timerId = Timer:schedule(function ()
            self:_onTick()
        end, self._timerInterval, 0)
        self:_onTick()
    end
end

-- 清除定时器
function M:__clearTimer()
    if self.__timerId then
        Timer:unschedule(self.__timerId)
        self.__timerId = false
    end
end

--------------------------------------------------
function M:_initObj()
    if self._obj then
        if self._fairyBatching then
            self._obj.fairyBatching = self._fairyBatching
        end
        self._obj.onAddedToStage:Add(function ()
            self:__onAddedToStage()
        end)
        self._obj.onRemovedFromStage:Add(function ()
            self:__onRemovedFromStage()
        end)
        self:_initUI()
    end
end

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



rawset(_G, "FWidget", M)
