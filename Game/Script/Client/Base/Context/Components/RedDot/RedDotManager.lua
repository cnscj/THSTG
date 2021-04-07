--[[
    利用前缀树实现
]]
local RedDotNode = require("Context.Components.RedDot.RedDotNode")
local M = class("RedDotManager")

function M:ctor()
    self._root = false

    self._touchFunc = function ( node )
        if node.callback then node.callback() end
    end
end

function M:register(callback, ...)
    local node = self:_insert(...)
    if node then
        node.callback = callback
    end
end

function M:unregister(...)
    local node = self:_search(...)
    if node then 
        node.callback = false
    end
end

function M:refresh(...)
    self._touch(self._touchFunc, ...)
end

--
function M:_createNode()
    local node = RedDotNode.new()
    return node 
end

function M:_insert(...)
    local argsNum = select("#", ...)
    if argsNum > 0 then
        local node = self:_getRoot()
        for i = 1, argsNum do
            local arg = select(i, ...)
            if not arg then return end
            
            node.children = node.children or {}
            local nextNode = node.children[arg]
            if not nextNode then
                nextNode = self:_createNode()
                nextNode.name = arg
                
                node.children[arg] = nextNode
            end

            node = nextNode
        end
        
        node._isEnd = true
        return node
    end

    return false
end

function M:_remove( ... )
    local argsNum = select("#", ...)
    if argsNum > 0 then
        local node = self:_getRoot()
        for i = 1, argsNum - 1 do
            local arg = select(i, ...)
            if not arg then return end

            local nextNode = false
            if node.children then nextNode = node.children[arg] end

            if not nextNode then return end
            node = nextNode
        end

        local arg = select(argsNum, ...)
        if node.children then node.children[arg] = nil end

    end
end

function M:_search( ... )
    local argsNum = select("#", ...)
    if argsNum > 0 then
        local node = self:_getRoot()
        for i = 1, argsNum do
            local arg = select(i, ...)
            if not arg then return false end

            local nextNode = false
            if node.children then nextNode = node.children[arg] end

            if not nextNode then return false end

            node = nextNode
        end

        if not node._isEnd then return false end
        return node
    end
    return false
end

function M:_touch(func, ... )
    local argsNum = select("#", ...)
    if argsNum > 0 then
        local node = self:_getRoot()
        local nodePath = {}
        for i = 1, argsNum - 1 do
            local arg = select(i, ...)
            if not arg then return end
            
            local nextNode = false
            if node.children then nextNode = node.children[arg] end

            if not nextNode then return false end
            node = nextNode

            if node._isEnd then
                table.insert(nodePath,node)
            end
        end

        --从后面向父节点触发
        if func then
            for i = #nodePath, 1 do 
                func(nodePath[i])
            end
        end

        return nodePath
    end
    return false
end

function M:_getRoot()
    self._root = self._root or self:_createNode()
    self._root._isEnd = false

    return self._root
end

return M