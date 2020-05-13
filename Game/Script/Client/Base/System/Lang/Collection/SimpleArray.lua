--------------------------动态数组类（本来设计为Array，现在被魔改成完全不是当初设想的样子，重新写）----------------------------------
--这个类基于lua自带的table实现，但不同的地方是：
--1. 动态数组会预分配一个比table默认大小要大很多的容量，这样减少resize次数
--2. 插入和删除并不会改变数组的容量，除非总长度已经大于预分配的容量，这时会自动扩容
--3. 只支持插入到第一个和最后一个位置，类似于队列，要保证数据的连续性
--4. 该类只适合那些创建一个就用很久的环境，临时变量或者频繁创建的地方最好用lua table

-----------------------------------------------这个类禁止修改！！！---------------------------------------------------------

local SimpleArray = simpleClass("SimpleArray")

local AUTO_EXPAND_FACTOR = 2

function SimpleArray:ctor(capcity)
    capcity = capcity or 10
    self._array = self:_allocate(capcity)  --data collection
    self._size = 0
end

function SimpleArray:size()
    return self._size
end

function SimpleArray:clear()
    for i = 1, #self._array do
        self._array[i] = false
    end
    self._size = 0
end

function SimpleArray:append(node)
    --扩容
    if self._size + 1 > #self._array then
        self:_expandSize()
    end

    --插入到这个位置
    self._array[self._size + 1] = node

    self._size = self._size + 1
end

function SimpleArray:remove(index)
    if self._size <= 0 or index > self._size then
        return nil
    end

    --移除
    local node = self._array[index]

    --把最后面的移过来
    self._array[index] = self._array[self._size]

    --再移除最后一个，用false占位
    self._array[self._size] = false

    self._size = self._size - 1

    return node
end

function SimpleArray:removeLast()
    return self:remove(self._size)
end

--不支持直接设置下标超过size+1的元素
function SimpleArray:set(newNode, index)
    -- 支持insertLast
    if index == self._size + 1 then
        if newNode ~= nil then
            self:append(newNode)
        end
        return true
    end

    if self._size <= 0 or index > self._size then
        return false
    end

    self._array[index] = newNode

    return true
end

function SimpleArray:get(index)
    if self._size <= 0 or index > self._size then
        return nil
    end

    return self._array[index]
end

--迭代器
function SimpleArray:ipairs()
    local i = 0
    return function()
        i = i + 1
        if i <= self._size then
            return i, self._array[i]
        end
    end
end

function SimpleArray:toString()
    local list = {}
    for i = 1, self._size do
        table.insert(list, tostring(self._array[i]))
    end
    return "{" .. table.concat(list, ", ") .. "}"
end


--自动扩容
function SimpleArray:_expandSize()
    -- print("SimpleArray:_expandSize(): try to set new capcity=", #self._array*AUTO_EXPAND_FACTOR)

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
function SimpleArray:_allocate(count)
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

rawset(_G, "SimpleArray", false)
_G.SimpleArray = SimpleArray