local M = class("FWindow", FView)

function M:ctor(obj,args)
    args = args or {}
    
    --是否是非模态的(模态窗口必须完成当前窗口才能继续)
    self._isModeless = false
end

return M