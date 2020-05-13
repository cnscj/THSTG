--------------------------最小堆辅助类----------------------------------
--  一个最小堆的辅助类，外部自己维护一个数组t，
--  如果不想外部自己维护数组，可以使用Heap.lua
-----------------------------------------------------------------------
local HeapHelper = simpleClass("HeapHelper")

HeapHelper.HEAP_TYPE_MIN = "min"
HeapHelper.HEAP_TYPE_MAX = "max"

--comparator: a compare function to sort the heap node, the function should return a number that is =0, >0 or <0
--heapType: "min" or "max", "min" is the default type
function HeapHelper:ctor( comparator, heapType )
    if type(comparator) == "function" then
        self._comparator = comparator
    else
        self._comparator = function(a, b) return 0 end
        printError("You must set a comparator!")
    end

    if heapType and string.lower(heapType) == HeapHelper.HEAP_TYPE_MAX then
        self._type = HeapHelper.HEAP_TYPE_MAX
    else
        self._type = HeapHelper.HEAP_TYPE_MIN
    end
end


--交换位置
function HeapHelper:_swap(t, from, to)
    if from == to then
        return
    end
    
    local tmp = t[from]
    t[from] = t[to]
    t[to] = tmp
end

--最小堆
function HeapHelper:_heapifyMin(t, i, heapSize)
    local l = i * 2  -- 左子节点
    local r = i * 2 + 1     -- 右子节点
    local min = i      -- 暂时定在Index的位置就是最小值

    -- 存在左结点，且左结点的值小于根结点的值，注意comparator用.号
    if l <= heapSize and self._comparator(t[l], t[min]) < 0 then
        min = l
    end
    
    -- 存在右结点，且右结点的值小于以上比较的较小值，注意comparator用.号
    if r <= heapSize and self._comparator(t[r], t[min]) < 0 then
        min = r
    end 
    
    -- 左右结点的值都大于根节点，直接return，不做任何操作
    if i == min then  
        return
    end
    
    -- 交换根节点和左右结点中最小的那个值，把根节点的值替换下去
    self:_swap(t, i, min)
    
    -- 由于替换后左右子树会被影响，所以要对受影响的子树再进行heapify
    self:_heapifyMin(t, min, heapSize)
end

--最大堆
function HeapHelper:_heapifyMax(t, i, heapSize)
    local l = i * 2  -- 左子节点
    local r = i * 2 + 1     -- 右子节点
    local max = i      -- 暂时定在Index的位置就是最大值

    -- 存在左结点，且左结点的值大于根结点的值，注意comparator用.号
    if l <= heapSize and self._comparator(t[l], t[max]) > 0 then
        max = l
    end
    
    -- 存在右结点，且右结点的值大于以上比较的较大值，注意comparator用.号
    if r <= heapSize and self._comparator(t[r], t[max]) > 0 then
        max = r
    end 
    
    -- 左右结点的值都小于根节点，直接return，不做任何操作
    if i == max then  
        return
    end
    
    -- 交换根节点和左右结点中最大的那个值，把根节点的值替换下去
    self:_swap(t, i, max)
    
    -- 由于替换后左右子树会被影响，所以要对受影响的子树再进行heapify
    self:_heapifyMax(t, max, heapSize)
end

--创建最小堆
function HeapHelper:_buildMinHeap(t)
    local size = #t
    local half = math.floor(size / 2)

    for i=half, 1, -1 do
        self:_heapifyMin(t, i, size)
    end
end

--创建最大堆.
function HeapHelper:_buildMaxHeap(t)
    local size = #t
    local half = math.floor(size / 2)

    for i=half, 1, -1 do
        self:_heapifyMax(t, i, size)
    end
end

function HeapHelper:_doBuildHeap(t)
    if self._type == HeapHelper.HEAP_TYPE_MIN then
        self:_buildMinHeap(t)
    else
        self:_buildMaxHeap(t)
    end
end


--长度.
function HeapHelper:size(t)
    return t and #t or 0
end

--根结点
function HeapHelper:peek(t)
    return (t and #t > 0) and t[1] or nil
end

--弹出根结点
function HeapHelper:pop(t)
    local size = #t
    if size == 0 then
        return nil
    end
    
    local result = t[1]

    if size == 1 then
        table.remove(t, 1)
        return result
    end

    --exchange the first and the last one, then rebuild heap
    self:quickRemove(t, 1)
    self:_doBuildHeap(t)

    return result
end

--添加一个结点
function HeapHelper:push(t, node)
    --add to last, then rebuild heap
    table.insert(t, node)
    self:_doBuildHeap(t)
end


function HeapHelper:quickRemove(t, i)
    --exchange the i and the last one, then remove the last one
    local size = self:size(t)
    self:_swap(t, i, size)
    table.remove(t, size)
end

function HeapHelper:doBuild(t)
    self:_doBuildHeap(t)
end

---------------------Extensions----------------------
--调整指定结点
function HeapHelper:heapifyAt(t, i)
    if self._type == Heap.HEAP_TYPE_MIN then
        self:_heapifyMin(t, i, self:size(t))
    else
        self:_heapifyMax(t, i, self:size(t))
    end
end
function HeapHelper:heapifyPeek(t)
    self:heapifyAt(t, 1)
end
function HeapHelper:heapifyTail(t)
    self:heapifyAt(t, self:size(t))
end

rawset(_G, "HeapHelper", HeapHelper)
