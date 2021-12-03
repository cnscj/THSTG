local M = class("FView",FWidget)

function M:ctor(obj,args)
    args = args or {}
    -- 参数
    self._args = args

    -- 被加到哪一层
    self._parentLayer = args.parentLayer or LayerDepth.Zero or 0

    -- view名字
    self._viewName = self.__cname

end


rawset(_G, "FView", M)