local M = class("MainUIView",FView)

function M:ctor()
    self._package = "MainUI"
    self._component = "MainUIView"

    self._loadMethod = false
end

return M