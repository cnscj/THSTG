--------------------------堆栈类----------------------------------
---@class Stack
local Stack = simpleClass("Stack")

function Stack:ctor()
    self._array = {}  --data collection
end

function Stack:size()
    return #self._array
end

function Stack:clear()
    self._array = {}
end

function Stack:peek()
    local size = self:size()
    if size > 0 then
        return self._array[size]
    else
        return nil
    end
end

function Stack:push(node)
    table.insert(self._array, node)
end

function Stack:pop()
    local size = self:size()
    if size > 0 then
        return table.remove(self._array)
    else
        return nil
    end
end

rawset(_G, "Stack", false)
_G.Stack = Stack