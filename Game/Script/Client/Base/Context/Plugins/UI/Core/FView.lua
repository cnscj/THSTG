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
----------------
-- 关闭
-- 打开用UIManager.openView，删除要用 打开用UIManager.closeView
-- 不能直接self:_doClose()
-- @param   isImmediate     #boolean       是否不播放任何动画直接关闭
-- @param   needDispose     #boolean       关闭后是否销毁
function M:doClose(isImmediate,needDispose)
    if self:getObj() then 
        if needDispose then
            self:dispose()
        else
            self:removeFromParent()
        end
    else
        self.__isCloseFlag = true
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

rawset(_G, "FView", M)