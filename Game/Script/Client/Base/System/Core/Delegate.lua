---@class Delegate
local M = {}

local metatable = {}

metatable.__add = function(self, func)
    if not func then
        return self
    end
    for i = 1, #self do
        if self[i] == func then
            return self
        end
    end
    self[#self + 1] = func
    return self
end

metatable.__sub = function(self, func)
    if not func then
        return self
    end
    for i = 1, #self do
        if self[i] == func then
            table.remove(self, i)
            return self
        end
    end
    return self
end

-- 因为可能在func中移除，所以逆序遍历
metatable.__call = function(self, ...)
    local i = #self
    while (i >= 0) do
        local func = self[i]
        if func then
            func(...)
        end
        i = i - 1
    end
end

local add = function(self, func)
    if not func then
        return self
    end
    for i = 1, #self do
        if self[i] == func then
            return self
        end
    end
    self[#self + 1] = func
    return self
end

local remove = function(self, func)
    if not func then
        return self
    end
    for i = 1, #self do
        if self[i] == func then
            table.remove(self, i)
            return self
        end
    end
    return self
end

local clear = function(self)
    for i = 1, #self do
        self[i] = nil
    end
end

function M.new()
    local delegate = {}
    setmetatable(delegate, metatable)
    delegate.clear = clear
    delegate.add = add
    delegate.remove = remove
    return delegate
end

Delegate = M