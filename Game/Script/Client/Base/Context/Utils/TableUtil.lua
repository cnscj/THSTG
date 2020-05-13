--[[
	更多请看Table.lua
]]
---@class TableUtil
local M = {}

-- 如果a和b的key值都有，a的值会覆盖b的值。
-- 得到新的tb，不改变原来的a和b。
function M.mergeTable(a, b, isInit)
    if isInit == nil then
        -- a = clone(a)
        b = clone(b)
    end
    if a == nil then
        return b
    end
    if type(b) == "table" and type(a) == "table" then
        for k, v in pairs(b) do
            b[k] = M.mergeTable(a[k], b[k], false)
        end
        return b
    else
        return a
    end
end

-- 如果a和b的key值都有，a的值会覆盖b的值。
-- 如果key值，b没有，a有，则将key值赋给新的tb，
-- 得到新的tb，不改变原来的a和b。
function M.mergeAllTable(a, b, isInit)
    if isInit == nil then
        a = clone(a)
        b = clone(b)
    end
    if a == nil then
        return b
    end
    if type(b) == "table" and type(a) == "table" then
        for k, v in pairs(b) do
            b[k] = M.mergeAllTable(a[k], b[k], false)
        end
        for k, v in pairs(a) do
            if b[k] == nil then
                b[k] = v
            end
        end
        return b
    else
        return a
    end
end

function M.appendTable(tab, ...)
    if type(tab) == "table" then
        local params = { ... }
        for i, list in ipairs(params) do
            for i, v in ipairs(list) do
                table.insert(tab, v)
            end
        end
    end
    return tab
end

--安全取得key内容
function M.safeGetValue(default, table, ...)
    if table then
        if type(table) == "table" then
            local value = table
            local argsNum = select("#", ...)
            for i = 1, argsNum do
                local arg = select(i, ...)
                value = value[arg]
                if value == nil then
                    return default
                end
            end
            return value
        end
    end
    return default
end

--安全设置值
function M.safeSetValue(value, table, ...)
    local params = { ... }
    if table then
        if type(table) == "table" then
            local tb = table
            if #params > 0 then
                for i = 1, #params - 1 do
                    tb = tb[params[i]]
                    if tb == nil then
                        return false
                    end
                end

                if tb[params[#params]] then
                    tb[params[#params]] = value
                    return true
                else
                    return false
                end
            else
                return false
            end
        end
    end
    return false
end

--强制设置值
function M.forceSetValue(value, table, ...)
    local params = { ... }
    if table then
        if type(table) == "table" then
            local tb = table
            if #params > 0 then
                for i = 1, #params - 1 do
                    if tb[params[i]] == nil then
                        tb[params[i]] = {}
                    end
                    tb = tb[params[i]]
                end
                tb[params[#params]] = value
                return true
            else
                return false
            end
        end
    end
    return false
end

--安全移除,解决在循环中移除造成的错位问题
function M.safeRemoveItem(list, item, removeAll)
    local rmCount = 0
    local defaultFunc = function(v)
        return v == item
    end

    if type(item) == "function" then
        defaultFunc = item
    end

    for i = 1, #list do
        if defaultFunc(list[i - rmCount]) then
            table.remove(list, i - rmCount)
            if removeAll then
                rmCount = rmCount + 1
            else
                break
            end
        end
    end
end

--pairs顺序遍历 table(按key从小到大遍历) 
--迭代器,desc为true为从大到小
function M.pairsByKeys(t, desc)
    local a = {}
    for n in pairs(t) do
        a[#a + 1] = n
    end
    table.sort(a, function(a, b)
        if desc then
            return (a > b)
        else
            return (a < b)
        end
    end)
    local i = 0
    return function()
        i = i + 1
        return a[i], t[a[i]]
    end
end

--有序数组二分法查找
--func返回-1,0,1表示在表条目(与字段无关,不过注意从小到大)的目标的左,目标,右
function M.searchByBinary(t, func)
    if type(t) == "table" and type(func) == "function" then
        local low, high = 1, #t
        local mid = -1
        while (low <= high) do
            mid = math.floor((low + high) / 2)
            if func(t[mid]) == 0 then
                return t[mid], mid
            elseif func(t[mid]) < 0 then
                high = mid - 1
            elseif func(t[mid]) > 0 then
                low = mid + 1
            end
        end
    end
    return nil, -1
end

--取得table真实长度
--如果你非常确定是数组,请用直接用#t
--如果不确定,就用这个
function M.getLength(t)
    t = t or {}
    local length = 0
    for _, _ in pairs(t) do
        length = length + 1
    end
    return length
end

--输出所有键
function M.keys2Array(tb)
    local array = {}
    for k, _ in pairs(tb) do
        table.insert(array, k)
    end
    return array
end

--[[
	转为数组
	{
		[a] = {...},
		[b] = {...},
		[c] = d,
	}
	==
	{
		{key = a, value = {...}},
		{key = b, value = {..}},
		{key = c, value = d}
	}
]]
function M.trans2Array(tb)
    local array = {}
    for k, v in pairs(tb) do
        table.insert(array, {
            key = k,
            value = v
        })
    end
    return array
end



--按键重组
--tb必须是数组
--[[
	如
	t={
		{a,b,c},

	}
	重组为
	regroupTable(t,"a","b","c")

	t={
		a={
			b={
				c = {...}
			}
		}
	},最后按数组方式插入
]]
function M.trans2Table(tb, ...)
    if type(tb) == "table" then
        local keys = { ... }
        if #keys > 0 then
            --层级往下递减
            local newTb = {}
            for k, v in pairs(tb) do
                local prevTb = newTb
                for i = 1, #keys - 1 do
                    local keyName = keys[i]
                    local key = v[keyName]
                    newTb[key] = newTb[key] or {}
                    prevTb = newTb[key]
                end
                --直接赋值还是插入v?
                prevTb[v[keys[#keys]]] = prevTb[v[keys[#keys]]] or {}
                table.insert(prevTb[v[keys[#keys]]], v)
            end
            return newTb
        else
            return tb
        end
    end
    return tb
end
---获得表中最大的key，key必须为数字
function M.getMaxArrKey(table)
    local max = false
    for key, value in pairs(table) do
        if not max or key > max then
            max = key
        end
    end
    return false
end

--时间换空间？
function M.clear(t)
    if type(t) == "table" then
        for key, _ in pairs(t) do
            t[key] = nil
        end
    end
end

function M.clearArray(t)
    for i = 1, #t do
        t[i] = nil
    end
end

rawset(_G, "TableUtil", false)
TableUtil = M