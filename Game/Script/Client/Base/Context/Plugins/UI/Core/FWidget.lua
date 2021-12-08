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

    self._eventListeners = {}
    self._timerInterval = false
    self._timerId = false

    self.__isLoading = false
end

function M:init(obj,args)
    self._root = self
    self._rootGO = obj and obj.displayObject.gameObject or false

    self:__setupObj()
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
        return
    end

    if string.isEmpty(self._package) or string.isEmpty(self._component) then
        printWarning(string.format( "self._package or self._component can't be empty"))
        return 
    end

    self.__isLoading = true
    UIPackageManager:loadPackage(self._package ,self._loadMethod, function ( packageWrap )
        self.__isLoading = false
        self:__loadPackageCallback(packageWrap)
        if onSuccess then onSuccess() end
    end,onFailed)
end

function M:__onAddedToStage( ... )
    self:__onEnter()
end

function M:_onRemovedFromStage()
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
    for _, event in ipairs(self._eventListeners) do
        Dispatcher.removeEventListener(event.name, event.listener, self)
    end
end

function M:__setupObj()
    self:_initObj()
    if self._obj then
        if self._fairyBatching then
            self._obj.fairyBatching = self._fairyBatching
        end
        self._obj.onAddedToStage:Add(function ()
            self:__onAddedToStage()
        end)
        self._obj.onRemovedFromStage:Add(function ()
            self:_onRemovedFromStage()
        end)
        self:_initUI()
    end
end

function M:__loadPackageCallback(packageWrap)
    self._obj = UIPackageManager:createObject(self._package ,self._component)
    if not self._obj then
        printError(string.format("Check that component %s is set to export", self._component))
        return 
    end 

    self._root = self
    self._rootGO = self._obj and self._obj.displayObject.gameObject or false

    self._obj.onAddedToStage:Add(function ()
        UIPackageManager:retainPackage(self._package)
    end)
    self._obj.onRemovedFromStage:Add(function ()
        UIPackageManager:releasePackage(self._package)
    end)

    self:__setupObj()
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
function M:_initObj()

end

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

rawset(_G, "FWidget", M)
