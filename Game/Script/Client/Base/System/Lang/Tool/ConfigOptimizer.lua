------------------------------------------------------------------
--该文件中的方法主要在优化配置表中使用
------------------------------------------------------------------

local _useOptimizedTable = false

local function useOptimizedTable()
	return _useOptimizedTable
end

-- 获取优化表的列元表
local function getColMetatable(tb)
	return {
		__index = function(t, k)
			local i = tb[k]
			if i then
				return t[i]
			end
		end
	}
end

-- 获取优化表的行元表
local function getRowMetatable(tb, mt)
	return {
		__index = function(t, k)
			local i = tb[k]
			if i then
				setmetatable(i, mt)
			end
			return i
		end
	}
end

-- 给优化表增加iter()迭代器方法
local function addIterFunc(tResult, tRow, mt)
	function tResult.iter()
		return function(t, k)
			local rk, rv
			if k then
				rk, rv = next(t, k)
			else
				rk, rv = next(t)
			end
			if rk then
				setmetatable(rv, mt)
			end
			return rk, rv
		end, tRow
	end
end

-- 获取优化表的行元表
local function getRowMetatableEx(tb, mtFunc)
	return {
		__index = function(t, k)
			local i = tb[k]
			if i and not getmetatable(i)  then
				setmetatable(i, mtFunc(i, k))
			end
			return i
		end
	}
end

-- 给优化表增加iter()迭代器方法
local function addIterFuncEx(tResult, tRow, mtFunc)
	function tResult.iter()
		return function(t, k)
			local rk, rv
			if k then
				rk, rv = next(t, k)
			else
				rk, rv = next(t)
			end
			if rk and not getmetatable(rv) then
				setmetatable(rv, mtFunc(rv, rk))
			end
			return rk, rv
		end, tRow
	end
end

-- 获取常规经过优化的表
-- @param tRow	#table	优化表中的行数据表
-- @param tColumn	#table	优化表中的列数据表
-- @return	#table	经过优化后的表，照样可以通过字段名访问值，并且可以通过iter迭代器遍历
local function getNormalConfigTable(tRow, tColumn)
	local t = {}
	local mtCol = getColMetatable(tColumn)
	local mtRow = getRowMetatable(tRow, mtCol)
	addIterFunc(t, tRow, mtCol)
	setmetatable(t, mtRow)
	return t
end

-- 获取特殊经过优化的表，暂时只用于物品表
-- @param tRow		#table		优化表中的行数据表
-- @param getMtFunc	#function	返回对应mt表的函数，格式：function getMtFunc(mtCol1, ...) return function(tb, key) end end
-- @param tCol1		#table		优化表中的列数据表1
-- @param ...		#table		优化表中的列数据表2-N
-- @return	#table	经过优化后的表，照样可以通过字段名访问值，并且可以通过iter迭代器遍历
local function getSpecialConfigTable(tRow, getMtFunc, tCol1, ...)
	local tCols = {tCol1, ...}
	local mtCols = {}
	for k, v in pairs(tCols) do
		mtCols[#mtCols + 1] = getColMetatable(v)
	end
	local t = {}
	local mtColFunc = getMtFunc(table.unpack(mtCols))
	local mtRow = getRowMetatableEx(tRow, mtColFunc)
	addIterFuncEx(t, tRow, mtColFunc)
	setmetatable(t, mtRow)
	return t
end


--添加迭代器，实际是在遍历所有列，再取出列对应的值
--self: tRow
--[[
用法：
	for k,v in ConfigOptimizer.iterRow(tRow)

	end
]]
local function iterRow(self)
	local index = 0
	return function(t)
		index = index + 1

		local column = rawget(t, "__tColumn")
		if column and index <= #column then
			local colName = column[index]
			return colName, t[colName]
		end
	end, self
end

--pairs迭代方法
local function pairsFunc(t, key)
	local column = rawget(t, "__tColumn")
	if column then
		--用当前的列名取出对应列的下标，再取下一个下标，返回对应的列名，依此迭代，直到下标超过最大列数
		local curIndex = key and column[key] or 0
		if curIndex then
			local nextIndex = curIndex + 1
			if nextIndex <= #column then
				local nextKey = column[nextIndex]
				return nextKey, t[nextKey]
			end
		end
	end
end

local configRowMeta = {
	__newindex = function( ... )
		if printError then
			printError("!!!!!!!!!!!!!!!!!!!!!!!!!!!Attempt to modify the read-only table!!!!!!!!!!!!!!!!!!!!!!!!!!!")
		else
			error( "Attempt to modify the read-only table" )
		end
	end,
	__index = function(t, key)
		local column = rawget(t, "__tColumn")

		--根据key获得值在数组中的index
		local i = column and column[key]
		if i then
			local value = rawget(t, i)
			--如果数组里值为空，取默认表里的值
			if value == nil then
				local defaults = rawget(t, "__tDefaults")
				if defaults then
					return defaults[key]  --这里用的是key-value形式
				end
			else
				return value
			end
		end
	end,
	__pairs = function(t, key)
       return pairsFunc, t, nil
    end,
}


--直接修改原始的tRow，不创建新的表
local function optimizeRow(tRow, tColumn, tDefaults)
	if type(tRow) ~= "table" or type(tColumn) ~= "table" or type(tDefaults) ~= "table" then
		print(0, "-----------------------------------------------------")
		print(0, "ERROR: Can't optimize the row, coz some of parameters are not a table!")
		print(0, "-----------------------------------------------------")
		print(0, debug.traceback())
		return
	end

	_useOptimizedTable = true

	--缓存起来，用来遍历和查找
	tRow["__tColumn"] = tColumn
	tRow["__tDefaults"] = tDefaults
	setmetatable(tRow, configRowMeta)

	return tRow
end


ConfigOptimizer = {
	getNormalConfigTable = getNormalConfigTable,
	getSpecialConfigTable = getSpecialConfigTable,
	-- addIterFunc = addIterFunc,
	-- getRowMetatable = getRowMetatable,
	-- getColMetatable = getColMetatable,

	useOptimizedTable = useOptimizedTable,
	optimizeRow = optimizeRow,
}
