local M = class("FWindow", FView)

function M:ctor(obj,args)
    --是否是非模态的(模态窗口必须完成当前窗口才能继续)
    self._isModeless = false
end

function M:init(obj,args)
    self._parentLayer = args and args.parentLayer or ViewLayer.Window or 0
end

return M