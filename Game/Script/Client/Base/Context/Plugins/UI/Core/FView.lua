local M = class("FView")

function M:ctor(args)
    args = args or {}
    -- 参数
    self._args = args
    -- [待子类覆盖] FGUI中的资源包名
    self._package = args.package or ""
    -- [待子类覆盖] 资源包中的组件
    self._component = args.component or ""
    -- 被加到哪一层
    self._parentLayer = args.parentLayer or LayerDepth.Zero

    -- view名字
    self._viewName = self.__cname
    ---ui根节点(Lua层的GComponet)
    self._root = false
    --显示对象根节点(Unity层的GameObject)
    self._rootGO = false
    ---ui父节点，如在窗口中可能对应窗口内的contentNode节点对象
    self._parent = false
    --事件监听
    self._eventListeners = {}
    --定时器
    self._timerInterval = false
    self._timerId = false
end

function M:toCreate()
    -- 创建时，先判断一下父节点
    if self._parent:isDisposed() then
        return
    end

    self:__readyPreloadResList()
end

-- 添加事件监听
function M:addEventListener(name, listener, listenerCaller, priority)
    Dispatcher.addEventListener(name, listener, listenerCaller, priority)
    table.insert(self._eventListeners, {name=name, listener=listener})
end
--


function M:_onTick( ... )
 
end

function M:_enter( ... )
    
end

function M:_exit( ... )
    
end

--

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


--
function M:__loadPackageCallback(...)
    self._rootGO = UIPackageManager:createObject(self._package ,self._component)
    self._root = FGUIUtil.createComp(self._rootGO)
    
end

function M:__readyPreloadResList()
    if string.isEmpty(self._package ) then
        return 
    end

    if not UIPackageManager:isLoadedPackage(self._package ) then
        UIPackageManager:loadPackage(self._package ,function ( ... )
            self:__loadPackageCallback(...)
        end)
    else
        self:__loadPackageCallback()
    end


end
rawset(_G, "FView", M)