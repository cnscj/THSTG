local M = class("FView",FWidget)

function M:ctor(obj,args)
    -- 参数
    self._args = args

    -- 被加到哪一层
    self._parentLayer = args and args.parentLayer or ViewLayer.Zero or 0

    -- view名字
    self._viewName = self.__cname

    --窗口动画
    self.__isPlayingAnimation = false
    self.__showTweenerHelper = false
    self.__hideTweenerHelper = false

    --窗口关闭flag
    self.__isCloseFlag = false

end

function M:init(obj,args)
    if args and args.parent then
        self._parent = args.parent
    else
        self._parent = UIManager:getParentLayer(self._parentLayer)
    end
end

function M:getViewName(...)
    return self._viewName
end

function M:closeView()
    UIManager:closeViewByView(self)
end

function M:isCanOpen(...)
   return self:_onCanOpen(...)
end
---------------
function M:_onEnter(...)
    self:super("FWidget", "_onEnter", ...)

    self:doShowAnimation(function ( ... )
        self:_onShow()
    end)
end

function M:_onExit(...)
    self:super("FWidget", "_onExit", ...)
end
----------------
-- 关闭
-- 打开用UIManager.openView，删除要用 打开用UIManager.closeView
-- 不能直接self:_doClose()
-- @param   isImmediate     #boolean       是否不播放任何动画直接关闭
-- @param   needDispose     #boolean       关闭后是否销毁
function M:doClose(isImmediate,needDispose)
    if self:getObj() then 
        if self:isDisposed() then
            printWarning("Has been removed.")
            return
        end

        if isImmediate then
            if needDispose then
                self:_onHide()
                self:dispose()
            else
                self:_onHide()
                self:removeFromParent()
            end
        else
            if needDispose then
                self:_onHide()
                self:doHideAnimation(function ( ... )
                    self:dispose()
                end)
            else
                self:_onHide()
                self:doHideAnimation(function ( ... )
                    self:removeFromParent()
                end)
            end
        end
    else
        self.__isCloseFlag = true
    end
end

function M:doShowAnimation(callback)
    self.__showTweenerHelper = self.__showTweenerHelper or self._root:getTransition("show")
    if self.__showTweenerHelper then
        self.__isPlayingAnimation = true
        self.__showTweenerHelper:Play(function()
            self.__isPlayingAnimation = false
            if callback then callback() end
        end)
    else
        if callback then callback() end
    end
end

function M:doHideAnimation(callback)
    self.__hideTweenerHelper = self.__hideTweenerHelper or self._root:getTransition("hide")
    if self.__hideTweenerHelper then
        self.__isPlayingAnimation = true
        self.__hideTweenerHelper:Play(function()
            self.__isPlayingAnimation = false
            if callback then callback() end
        end)
    else
        if callback then callback() end
    end
end


function M:isClosed( ... )
    return self.__isCloseFlag
end

----------------
--[[下面函数由子类覆写]]
--是否能够打开
function M:_onCanOpen(...)
    return true
end

--show动画播放结束执行
function M:_onShow( ... )
 
end

--hide动画播放结束执行
function M:_onHide( ... )

end

rawset(_G, "FView", M)