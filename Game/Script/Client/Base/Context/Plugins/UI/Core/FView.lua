local M = class("FView",FWidget)

function M:ctor(obj,args)
    args = args or {}
    -- 参数
    self._args = args

    -- 被加到哪一层
    self._parentLayer = args.parentLayer or ViewLayer.Zero or 0

    -- view名字
    self._viewName = self.__cname
end

function M:init(obj,args)
    args = args or {}

    if args.parent then
        self._parent = args.parent
    else
        self._parent = UIManager:getParentLayer(self._parentLayer)
    end
end

function M:getViewName( ... )
    return self._viewName
end

function M:closeView()
    UIManager:closeViewByView(self)
end

function M:isCanOpen(...)
   return self:OnCanOpen(...)
end

--[[下面函数由子类覆写]]
--是否能够打开
function M:OnCanOpen(...)
    return true
end

rawset(_G, "FView", M)