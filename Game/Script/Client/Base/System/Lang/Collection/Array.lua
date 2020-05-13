--------------------------动态数组类----------------------------------
--这个类基于lua自带的table实现，但不同的地方是：
--1. 动态数组会预分配一个比table默认大小要大很多的容量，这样减少resize次数
--2. 插入和删除并不会改变数组的容量，除非总长度已经大于预分配的容量，这时会自动扩容
--3. 只支持插入到第一个和最后一个位置，类似于队列，要保证数据的连续性
--4. 该类只适合那些创建一个就用很久的环境，临时变量或者频繁创建的地方最好用lua table
---@class Array
local Array = simpleClass("Array")

local AUTO_EXPAND_FACTOR = 2

function Array:ctor(capcity)
    capcity = capcity or 10
    self._array = self:_allocate(capcity)  --data collection
    self._size = 0
end

function Array:size()
    return self._size
end

function Array:clear()
    for i = 1, #self._array do
        self._array[i] = false
    end
    self._size = 0
end

function Array:insertFirst(node)
    self:insert(node, 1)
end

function Array:insertLast(node)
    self:insert(node, self._size + 1)
end

function Array:removeFirst()
    return self:remove(1)
end

function Array:removeLast()
    return self:remove(self._size)
end

function Array:remove(index)
    if self._size <= 0 or index > self._size then
        return nil
    end

    --移除
    local node = self._array[index]
    self._array[index] = false

    --把后面的移到前面来
    for i = index + 1, #self._array do
        self._array[i - 1] = self._array[i]
    end

    --移除最后一个
    self._array[self._size] = false

    self._size = self._size - 1

    return node
end

--把最后一个移到index位置，再把最后一个移除
function Array:quickRemove(index)
    if self._size <= 0 or index > self._size then
        return nil
    end

    --移除
    local node = self._array[index]

    --把最后面的移过来
    self._array[index] = self._array[self._size]

    --再移除
    self._array[self._size] = false

    self._size = self._size - 1

    return node
end

--不支持直接设置下标超过size+1的元素
function Array:set(newNode, index)
    -- 支持insertLast
    if index == self._size + 1 then
        self:insertLast(newNode)
        return true
    end

    if self._size <= 0 or index > self._size then
        return false
    end

    self._array[index] = newNode

    return true
end

function Array:get(index)
    if self._size <= 0 or index > self._size then
        return nil
    end

    return self._array[index]
end

--迭代器
function Array:ipairs()
    local i = 0
    return function()
        i = i + 1
        if i <= self._size then
            return i, self._array[i]
        end
    end
end

function Array:toString()
    local str = "{"
    for i = 1, self._size do
        str = str .. tostring(self._array[i]) .. ","
    end
    str = str .. "}"
    return str
end

---------------------------------扩展--------------------------------
function Array:find(element)
    for i = 1, self._size do
        if element == self._array[i] then
            return i
        end
    end
    return false
end

function Array:removeElement(element)
    local index = self:find(element)
    if index then
        self:remove(index)
        return true
    end
    return false
end

--不支持插入到指定位置，
function Array:insert(node, index)
    --扩容
    local realSize = #self._array
    if index > AUTO_EXPAND_FACTOR * realSize then
        printWarning("index=", index)
        printTraceback()
        error(string.format("不支持insert进超过%d倍容量的位置!", AUTO_EXPAND_FACTOR))
        return
    end

    if self._size + 1 > realSize then
        self:_expandSize()
    end

    --把index和后面的都往后移一位
    for i = self._size, index, -1 do
        self._array[i + 1] = self._array[i]
    end

    --插入到这个位置
    self._array[index] = node

    self._size = self._size + 1
end


--自动扩容
function Array:_expandSize()
    -- print("Array:_expandSize(): try to set new capcity=", #self._array*AUTO_EXPAND_FACTOR)

    local oldArray = self._array
    self._array = self:_allocate(#self._array * AUTO_EXPAND_FACTOR)

    if #self._array <= self._size then
        self._array = oldArray
        error("自动扩容失败！")
    else
        -- print("copy old array to new array, new capcity = ", #self._array)
        --老数据复制到新数组里去
        for i = 1, #oldArray do
            self._array[i] = oldArray[i]
        end
    end
    oldArray = nil
end

--预分配一定量的位置，防止数组频繁的动态扩容和拷贝数据。
function Array:_allocate(count)
    count = count or 10

    if count <= 10 then
        return {
            false, false, false, false, false, false, false, false, false, false,
        }
    elseif count <= 50 then
        return {
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        }
    elseif count <= 150 then
        return {
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,

            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        }
    elseif count <= 400 then
        return {
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,

            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,

            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,

            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        }
    else
        print(string.format("[WARNING] 数组容量大于400个，确认是否需要这么多？"))

        local arr = self:_allocate(400)
        for i=1, count-400 do
            table.insert(arr, false)
        end
        return arr
    end
end

rawset(_G, "Array", false)
_G.Array = Array