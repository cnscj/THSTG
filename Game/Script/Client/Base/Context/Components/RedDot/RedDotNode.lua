local M = class("RedDotNode")
function M:ctor()
    self.name = false
    self.callback = false   --回调
    self.children = false

    self.data = false

    self._isEnd = false     --是否算一个完整节点
end


return M