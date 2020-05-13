--------------------------最小/大堆----------------------------------
--  一个最小堆和最大堆的类，设置heapType来区别，每种堆有自己的comparator
--------------------------------------------------------------------
local Heap = simpleClass("Heap")

Heap.HEAP_TYPE_MIN = "min"
Heap.HEAP_TYPE_MAX = "max"

--comparator: a compare function to sort the heap node, the function should return a number that is 0, >0 or <0
--heapType: "min" or "max", "min" is the default type
function Heap:ctor( comparator, heapType )
    self._array = {}  --data collection

    if type(comparator) == "function" then
        self._comparator = comparator
    else
        self._comparator = function(a, b) return 0 end
        printError("You must set a comparator!")
    end

    if heapType and string.lower(heapType) == Heap.HEAP_TYPE_MAX then
        self._type = Heap.HEAP_TYPE_MAX
    else
        self._type = Heap.HEAP_TYPE_MIN
    end
end

function Heap:setComparator(comp, heapType)
    if type(comparator) == "function" then
        self._comparator = comp

        if heapType then
            if string.lower(heapType) == Heap.HEAP_TYPE_MAX then
                self._type = Heap.HEAP_TYPE_MAX
            else
                self._type = Heap.HEAP_TYPE_MIN
            end
        end

        self:_doBuildHeap()
    else
        printError("Invalid comparator!")
    end
end


--交换位置
function Heap:_swap(from, to)
    if from == to then
        return
    end

    local tmp = self._array[from]
    self._array[from] = self._array[to]
    self._array[to] = tmp
end

--最小堆
function Heap:_heapifyMin(i, heapSize)
    local l = i * 2  -- 左子节点
    local r = i * 2 + 1     -- 右子节点
    local min = i      -- 暂时定在Index的位置就是最小值

    -- 存在左结点，且左结点的值小于根结点的值，注意comparator用.号
    if l <= heapSize and self._comparator(self._array[l], self._array[min]) < 0 then
        min = l
    end
    
    -- 存在右结点，且右结点的值小于以上比较的较小值，注意comparator用.号
    if r <= heapSize and self._comparator(self._array[r], self._array[min]) < 0 then
        min = r
    end 
    
    -- 左右结点的值都大于根节点，直接return，不做任何操作
    if i == min then  
        return
    end
    
    -- 交换根节点和左右结点中最小的那个值，把根节点的值替换下去
    self:_swap(i, min)
    
    -- 由于替换后左右子树会被影响，所以要对受影响的子树再进行heapifyMin
    self:_heapifyMin(min, heapSize)
end

--最大堆
function Heap:_heapifyMax(i, heapSize)
    local l = i * 2  -- 左子节点
    local r = i * 2 + 1     -- 右子节点
    local max = i      -- 暂时定在Index的位置就是最大值

    -- 存在左结点，且左结点的值大于根结点的值，注意comparator用.号
    if l <= heapSize and self._comparator(self._array[l], self._array[max]) > 0 then
        max = l
    end
    
    -- 存在右结点，且右结点的值大于以上比较的较大值，注意comparator用.号
    if r <= heapSize and self._comparator(self._array[r], self._array[max]) > 0 then
        max = r
    end 
    
    -- 左右结点的值都小于根节点，直接return，不做任何操作
    if i == max then  
        return
    end
    
    -- 交换根节点和左右结点中最大的那个值，把根节点的值替换下去
    self:_swap(i, max)
    
    -- 由于替换后左右子树会被影响，所以要对受影响的子树再进行heapify
    self:_heapifyMax(max, heapSize)
end

--创建最小堆
function Heap:_buildMinHeap()
    local size = #self._array
    local half = math.floor(size / 2)

    for i=half, 1, -1 do
        self:_heapifyMin(i, size)
    end
end

--创建最大堆.
function Heap:_buildMaxHeap()
    local size = #self._array
    local half = math.floor(size / 2)

    for i=half, 1, -1 do
        self:_heapifyMax(i, size)
    end
end

function Heap:_doBuildHeap()
    if self._type == Heap.HEAP_TYPE_MIN then
        self:_buildMinHeap()
    else
        self:_buildMaxHeap()
    end
end


--长度.
function Heap:size()
    return self._array and #self._array or 0
end

--根结点
function Heap:peek()
    return (self._array and #self._array > 0) and self._array[1] or nil
end

--弹出根结点
function Heap:pop()
    local size = #self._array
    if size == 0 then
        return nil
    end
    
    local result = self._array[1]

    if size == 1 then
        table.remove(self._array, 1)
        return result
    end

    --exchange the first and the last one, then rebuild heap
    self:quickRemove(1)
    self:_doBuildHeap()

    return result
end

--添加一个结点
function Heap:push(node)
    --add to last, then rebuild heap
    table.insert(self._array, node)
    self:_doBuildHeap()
end

--清空
function Heap:clear()
    self._array = {}
end

function Heap:toArray()
    local ret = {}
    for i,v in ipairs(self._array) do
        table.insert(ret, v)
    end
    return ret
end

--快速移除一个结点
function Heap:quickRemove(i)
    --exchange the i and the last one, then remove the last one
    local size = self:size()
    self:_swap(i, size)
    return table.remove(self._array, size)
end

--手动重建堆
function Heap:doBuild()
    self:_doBuildHeap()
end

---------------------Extensions----------------------
--调整指定结点
function Heap:heapifyAt(i)
    if self._type == Heap.HEAP_TYPE_MIN then
        self:_heapifyMin(i, self:size())
    else
        self:_heapifyMax(i, self:size())
    end
end
function Heap:heapifyPeek()
    self:heapifyAt(1)
end

--移除某些结点.
function Heap:removeWhere(func)
    local count = 0
    local ret = {}

    for i=#self._array, 1, -1 do
        if func( self._array[i] ) then
            table.insert(ret, self:quickRemove(i))
            count = count + 1
        end
    end

    if count > 0 then
        self:_doBuildHeap()
    end

    return count, ret
end

rawset(_G, "Heap", Heap)
