--------------------------单端队列类----------------------------------
-- 数据保存在Queue table内部，维护一个head指针和值数量，可以轻松访问到队列的首尾
---------------------------------------------------------------------
---@class Queue
local M = class("Queue")

function M:ctor()
    self._tab = {}  --data collection
    self._head = 1
    self._size = 0
end

function M:size()
    return self._size
end

function M:clear()
    for i = self._head, self._head + self._size - 1 do
        self._tab[i] = nil
    end
    self._head = 1
    self._size = 0
end

function M:insertHead(value)
    self:enqueue(value)
    local i = self._head + self._size - 1
    while (i > self._head) do
        local value = self._tab[i - 1]
        self._tab[i - 1] = self._tab[i]
        self._tab[i] = value
        i = i - 1
    end
end

function M:contains(value)
    for i = self._head, self._head + self._size - 1 do
        if self._tab[i] == value then
            return true
        end
    end
    return false
end

function M:enqueue(value)
    self._tab[self._head + self._size] = value
    self._size = self._size + 1
    return self._size
end

function M:get(index)
    return self._tab[self._head + index - 1]
end

function M:find(element)
    for i = 1, self._size do
        if element == self._tab[i] then
            return i
        end
    end
    return false
end

function M:remove(index)
    if self._size == 0 then
        return nil
    end

    local ret = self._tab[self._head]
    self._tab[self._head] = nil
    table.remove(self._tab, self._head + index - 1)
    self._size = self._size - 1

    return ret
end

function M:removeElement(element)
    local index = self:find(element)
    if index then
        self:remove(index)

        return true
    end
    return false
end

function M:peek()
    if self._size == 0 then
        return nil
    end
    return self._tab[self._head]
end

function M:dequeue()
    if self._size == 0 then
        return nil
    end

    local ret = self._tab[self._head]
    self._tab[self._head] = nil
    self._head = self._head + 1
    self._size = self._size - 1

    return ret
end

rawset(_G, "Queue", false)
Queue = M
