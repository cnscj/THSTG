local M = class("RedDotNode")
function M:ctor()
    self.name = false
    self.callbacks = {}   --回调
    self.children = false

    self.data = false

    self._isEnd = false     --是否算一个结束点
end


rawset(_G, "RedDotNode", false)
RedDotNode = M