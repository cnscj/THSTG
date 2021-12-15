function table.nums(t)
    local count = 0
    for k, v in pairs(t) do
        count = count + 1
    end
    return count
end

function table.isArray(tab)
    if type(tab) ~= "table" then
        return false
    end

    local n = #tab
    for i, v in pairs(tab) do
        if type(i) ~= "number" then
            return false
        end

        if i > n then
            return false
        end
    end

    return true
end

function table.reverse(tab)
    if not table.isArray(tab) then
        return tab
    end
    local tmp = {}
    for i = #tab, 1, -1 do
        table.insert(tmp, tab[i])
    end
    return tmp
end

function table.keys(tab)
    local keys = {}
    for k, v in pairs(tab) do
        keys[#keys + 1] = k
    end
    return keys
end

function table.values(tab)
    local values = {}
    for k, v in pairs(tab) do
        values[#values + 1] = v
    end
    return values
end

function table.merge(dest, src)
    for k, v in pairs(src) do
        dest[k] = v
    end
end

function table.insertTo(dest, src, begin)
    begin = checkint(begin)
    if begin <= 0 then
        begin = #dest + 1
    end

    local len = #src
    for i = 0, len - 1 do
        dest[i + begin] = src[i + 1]
    end
end

function table.indexOf(array, value, begin)
    for i = begin or 1, #array do
        if array[i] == value then
            return i
        end
    end
    return false
end

function table.keyOf(tab, value)
    for k, v in pairs(tab) do
        if v == value then
            return k
        end
    end
    return nil
end

function table.removeByValue(array, value, removeall)
    local c, i, max = 0, 1, #array
    while i <= max do
        if array[i] == value then
            table.remove(array, i)
            c = c + 1
            i = i - 1
            max = max - 1
            if not removeall then
                break
            end
        end
        i = i + 1
    end
    return c
end

function table.map(t, fn)
    for k, v in pairs(t) do
        t[k] = fn(v, k)
    end
end

function table.walk(t, fn)
    for k, v in pairs(t) do
        fn(v, k)
    end
end

function table.filter(t, fn)
    for k, v in pairs(t) do
        if not fn(v, k) then
            t[k] = nil
        end
    end
end

function table.unique(t, bArray)
    local check = {}
    local n = {}
    local idx = 1
    for k, v in pairs(t) do
        if not check[v] then
            if bArray then
                n[idx] = v
                idx = idx + 1
            else
                n[k] = v
            end
            check[v] = true
        end
    end
    return n
end

--[[
    将b中键值对的值替换为a的值（如果a中存在的该键的话），返回b，表长度不变
]]
function table.mergeA2B(a, b)
    if type(b) == "table" and type(a) == "table" then
        for k, v in pairs(b) do
            b[k] = table.mergeA2B(a[k], b[k])
        end
        return b
    end
    return a
end

--[[
    将a中键值对覆盖到b中，返回b，表长度为ab合集长度
]]
function table.unionA2B(a, b)
    if type(b) == "table" and type(a) == "table" then
        for k, v in pairs(a) do
            --跟mergeA2B方法的差别就在这
            b[k] = table.unionA2B(a[k], b[k])
        end
        return b
    end
    return a
end

function table.tostring(tb, deep)
    deep = deep or 1;
    local vlist = {};
    for k, v in pairs(tb) do
        local key = (type(k) == "number" and ("[" .. k .. "]=") or ('[' .. string.format("%q", k) .. ']='));
        local value;
        local typeV = type(v)
        if typeV == "number" then
            value = v;
        elseif typeV == "string" then
            value = string.format("%q", v);
        elseif typeV == "table" then
            value = table.tostring(v, deep + 1);
        elseif typeV == "boolean" then
            value = tostring(v);
        else
            error("table.tostring error! Only support base type!")
        end
        table.insert(vlist, key .. value);
    end
    return "{" .. table.concat(vlist, ",") .. "}";
end



--随机挑选一个
function table.randPick(tab)
    local tmpKeyT = {}
    local n = 1
    for k in pairs(tab) do
        tmpKeyT[n] = k
        n = n + 1
    end
    return tab[tmpKeyT[math.random(1, n - 1)]]
end

--随机排序
function table.randSort(tab)
    local sortTbl = table.deepcopy(tab)
    local totalNum = #sortTbl
    for i = 1, totalNum do
        local index = math.random(1, totalNum)
        local temp = sortTbl[i]
        sortTbl[i] = sortTbl[index]
        sortTbl[index] = temp
    end
    return sortTbl
end

function table.tablecat(dest, orig)
    if type(orig) == 'table' and type(dest) == 'table' then
        for _, data in ipairs(orig) do
            table.insert(dest, data)
        end
    end
end

function table.sub(tbl, beginIdx, endIdx)
    beginIdx = beginIdx or 0
    endIdx = endIdx or math.huge

    local dest = {}
    local index = 0
    for _, data in pairs(tbl) do
        index = index + 1
        if index >= beginIdx then
            table.insert(dest, data)
            if endIdx <= index then
                break
            end
        end
    end
    return dest
end

function table.equal(tbl1, tbl2)
    if type(tbl1) == 'table' and type(tbl2) == 'table' then
        local count = 0
        for k, v in pairs(tbl1) do
            if not table.equal(v, tbl2[k]) then
                return false
            end
            count = count + 1
        end
        return count == table.nums(tbl2)
    end
    return tbl1 == tbl2
end

function table.mergeTb(...)
    local all = { ... }
    local t = {}
    for _, array in ipairs(all) do
        for _, v in ipairs(array) do
            table.insert(t, v)
        end
    end
    return t
end

function table.mergeDict(...)
    local all = { ... }
    local t = {}
    for _, array in ipairs(all) do
        for k, v in pairs(array) do
            t[k] = v
        end
    end
    return t
end
---字段转换成数组
function table.dictToTb(array)
    local t = {}
    for _, v in pairs(array) do
        table.insert(t, v)
    end
    return t
end


-- 只在测试用
function table.def2Table(def)
    local t = {}
    for k, v in pairs(def) do
        table.insert(t, { key = k, val = v })
    end
    table.sort(t, function(a, b)
        return a.val < b.val
    end)
    return t
end

--[[
    patch表里的字段覆盖origin表里的，但是又不污染origin表.
    注意：pairs时，只会遍历patch表自己的key,value

    local info = { a = 1, b = 2}

    local tb = table.wrap({
        a = 100
    },
    info)

    return tb
]]
function table.wrap(patch, origin)
    if not origin or type(origin) ~= "table" then
        return patch
    end

    patch = patch or {}
    setmetatable(patch, { __index = origin })
    return patch
end

-- 查看表tbl中是否存在键-key
function table.hasKey(tbl, key)
    if tbl == nil then
        return false
    end
    for k, v in pairs(tbl) do
        if k == key then
            return true
        end
    end
    return false
end

--查看表tbl中是否存在值value
function table.hasValue(tbl, value)
    if tbl == nil then
        return false
    end
    for k, v in pairs(tbl) do
        if v == value then
            return true
        end
    end
    return false
end

--table洗牌
function table.shuffle(list)
    local result = {}
    math.newrandomseed()
    local count = #list
    for i=1, count do
        local pick = math.random(1, #list)
        result[#result + 1] = list[pick]
        table.remove(list, pick)
    end
    return result
end

-- if __DEBUG__ then
--     -- 暂时禁用所有clone
--     local __clone = clone
--     function clone(t)
--         local str = getTraceback()
--         Cache.testCache:addTracebackCount(str)
--         return __clone(t)
--     end
-- end

-- expensive function!!!
function table.deepcopy(orig)
    local lookup_table = {}
    local function _copy(orig)
        if type(orig) ~= "table" then
            return orig
        elseif lookup_table[orig] then
            return lookup_table[orig]
        end
        local newObject = {}
        lookup_table[orig] = newObject
        for key, value in pairs(orig) do
            newObject[_copy(key)] = _copy(value)
        end
        return setmetatable(newObject, getmetatable(orig))
    end
    return _copy(orig)
end

local ReadOnly_Empty_Table = false
local ReadOnly_Empty_Metatable = {
    __newindex = function()
        printError( "不能给只读的空表添加元素" )
    end
}
function table.getReadOnlyEmptyTable()
    if not ReadOnly_Empty_Table then
        ReadOnly_Empty_Table = {}
        setmetatable(ReadOnly_Empty_Table, ReadOnly_Empty_Metatable)
    end
    return ReadOnly_Empty_Table
end

-- function table.setReadOnlyTable(tab)
--     setmetatable(tab, ReadOnly_Empty_Metatable)
-- end
table.empty = table.getReadOnlyEmptyTable()
