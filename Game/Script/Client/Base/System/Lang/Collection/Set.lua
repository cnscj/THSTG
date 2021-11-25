--------------------------集合类----------------------------------

local Set = simpleClass("Set")

function Set:ctor(t)
    self._tab = {}  --data collection
    self._size = 0

    t = t or {}
    for _, v in pairs(t) do
        self._tab[v] = true
    end
end

function Set:insert( element )
    if not element then
        return
    end

    if not self._tab[element] then
        self._tab[element] = true
        self._size = self._size + 1
    end
end

function Set:remove( element)
    if not element or not self._tab[element] then
        return
    end

    self._tab[element] = nil
    self._size = self._size - 1
end

function Set:contains( element )
    if not element then
        return false
    end
    return self._tab[element]
end

function Set:size()
    return self._size
end

function Set:iter()
    return function (t,k)
        local rk, rv = next(t,k)
        return rk,rv
    end,self._tab
end

function Set:clear()
    for ele,v in pairs(self._tab) do
        self._tab[ele] = nil
    end
    self._tab = {}
    self._size = 0
end

function Set:toArray()
    local ret = {}
    for i=1,self._size do
        ret[i] = true
    end

    local index = 1
    for ele, _ in pairs(self._tab) do
        ret[index] = ele
        index = index + 1
    end
    return ret
end


function Set.union(a, b)
    local ret = Set.new()

    if a then
        for i = 1, #a._tab do
            local ele = a._tab[i]
            ret:insert(ele)
        end
    end
    
    if b then
        for i = 1, #b._tab do
            local ele = b._tab[i]
            ret:insert(ele)
        end
    end
    
    return ret
end

function Set.intersect(a,b)
    local ret = Set.new()
    
    if a and b then
        for i = 1, #a._tab do
            local ele = a._tab[i]

            if b:contains(ele) then
                ret:insert(ele)
            end
        end
    end
    
    return ret
end

rawset(_G, "Set", Set)
