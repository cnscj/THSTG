--[[
    利用前缀树实现
]]
local M = class("RedDotManager")

function M:ctor()
    self._root = false

    self._touchFunc = function ( node )
        if node.callbacks then 
            for _,callback in pairs(node.callbacks) do 
               if callback then callback() end
            end
        end
    end
end

function M:register(callback, ...)
    local node = self:_insert(...)
    if node then
        table.insert(node.callbacks, callback)
    end
end

function M:unregister(callback, ...)
    local node = self:_search(...)
    if node then 
        table.remove(node.callbacks, callback)
        node.data = false
        if not next(node.callbacks) then self:remove(...) end
    end
end

function M:update(...)
    self:_touch(self._touchFunc, ...)
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

--直接销毁某个节点
function M:_destroy( ... )
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

        --从父节点中移除
        local arg = select(argsNum, ...)
        if node.children then node.children[arg] = nil end

    end
end

--移除节点:回溯移除其父节点
function M:remove( ... )
    local argsNum = select("#", ...)
    if argsNum > 0 then
        local node = self:_getRoot()
        local pathNodes = {}
        table.insert(pathNodes,node)
        for i = 1, argsNum do
            local arg = select(i, ...)
            if not arg then return end

            local nextNode = false
            if node.children then nextNode = node.children[arg] end

            if not nextNode then return end
            node = nextNode

            table.insert(pathNodes,node)
        end

        for i = #pathNodes - 1, 1, -1  do 
            local node = pathNodes[i]
            local nextNode = pathNodes[i + 1]
            
            if not nextNode.children or not next(nextNode.children) then
                if not nextNode._isEnd or nextNode == pathNodes[#pathNodes] then
                    node.children[nextNode.name] = nil
                    if not next(node.children) then node.children = false end
                end
            else
                if nextNode == pathNodes[#pathNodes] then
                    nextNode._isEnd = false
                end
                return
            end

        end

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
        local pathNodes = {}
        for i = 1, argsNum do
            local arg = select(i, ...)
            if not arg then return end
            
            local nextNode = false
            if node.children then nextNode = node.children[arg] end

            if not nextNode then return end
            node = nextNode

            table.insert(pathNodes,node)
        end

        --从后面向父节点触发
        if func then
            for i = #pathNodes, 1,-1 do 
                local node = pathNodes[i]
                if node._isEnd then
                    func(node)
                end
            end
        end
        return nodeList
    end
    return false
end

function M:_getRoot()
    self._root = self._root or self:_createNode()
    self._root._isEnd = false

    return self._root
end

function M:clear()
    self._root = false
end

rawset(_G, "RedDotManager", false)
RedDotManager = M.new()