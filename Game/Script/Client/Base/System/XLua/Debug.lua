-- 打印 控件创建和点击回调堆栈
__PRINT_TRACK__ = __PRINT_TRACK__ or true

-- 是否打印 traceback
__PRINT_TRACE_BACK__ = __PRINT_TRACE_BACK__ or true

--日志级别
__LUA_LOG_LEVEL__ = __LUA_LOG_LEVEL__ or 1

--输出类型
__LUA_LOG_LEVEL__ = __LUA_LOG_LEVEL__ or 0

--输出文件所在行
__PRINT_WITH_FILE_LINE__ = __PRINT_WITH_FILE_LINE__  or false

--日志级别
local LOG_LEVEL_INFO = 1
local LOG_LEVEL_WARNING = 2
local LOG_LEVEL_ERROR = 3
local LOG_LEVEL_NONE = 4

local Debug = CSharp.Debug--换成自己的日志类 CSharp.Logger
local logFunc = Debug.Log or print
local logWarningFunc = Debug.LogWarning or print
local logErrorFunc = Debug.LogError or print


function setLogLevel(level)
	__LUA_LOG_LEVEL__ = level
end
function setCSharpLogLevel(level)
	if level >= 0 then
		CSharp.Logger.logLevel = level
	end
end

-- 将 Table 转换成 string 返回
function table2String(rootT)
	if type(rootT) ~= "table" then 
		return tostring(rootT) 
	end

	local temp = {
		"{\n",
	}
	local function _table2String(t, depth)
		if type(depth) == "number" then
			depth = depth + 1
		else
			depth = 1
		end
		local indent = ""
		for i = 1, depth do
			indent = indent .. "    "
		end

		for k, v in pairs(t) do
			local key = tostring(k)
			if tonumber(key) then
				key = "["..key.."]"
			end
			local typeV = type(v)
			if typeV == "table" then
				table.insert(temp, indent..key.." = {\n")
				_table2String(v, depth)
				table.insert(temp, indent.."},\n")
			elseif typeV == "string" then
				--双引号转义
				local strVal = tostring(v)
				strVal = string.gsub(strVal,"\"","\\\"")
				table.insert(temp, string.format("%s%s = \"%s\",\n", indent, key, strVal))
			else
				table.insert(temp, string.format("%s%s = %s,\n", indent, key, tostring(v)))
			end
		end
	end
	_table2String(rootT)
	table.insert(temp, "}")
	return table.concat(temp)
end

function string2Table(str)
    if str == nil or type(str) ~= "string" then
        return
    end
	
	--函数解析不了
	str = string.gsub(str,"(%w+) = function: (%w+)(,?)","")

    return load(string.format("return %s",str))()
end

function print(...)
	if __LUA_LOG_LEVEL__ > LOG_LEVEL_INFO then
		return
	end

	local args = {...}
	local count = #args
	if count <= 0 then
		return 
	end

	local fromIndex = 1
	if __PRINT_TYPE__ > 0 then
		fromIndex = 2
		if args[1] ~= __PRINT_TYPE__ then
			return 
		end
	end

	local msg = ""
	if __PRINT_WITH_FILE_LINE__ then
		local traceback = string.split(debug.traceback("", 3), "\n")
		local tb = traceback[2]
		msg = string.format("print from: %s\n", string.trim(tb))
	end
	for i = fromIndex, count do
		msg = string.format("%s%s ", msg, tostring(args[i]))
	end
	logFunc(msg)
end

--始终打印，方便临时测试，测试完自己注释掉。
function log( ... )
	if __LUA_LOG_LEVEL__ > LOG_LEVEL_INFO then
		return
	end
	print(__PRINT_TYPE__, ...)
end

function printWarning(...)
	if __LUA_LOG_LEVEL__ > LOG_LEVEL_WARNING then
		return
	end

	local args = {...}
	local count = #args
	if count <= 0 then return end

	--警告忽略__PRINT_TYPE__
	
	local msg = ""
	for i = 1, count do
		msg = string.format("%s%s ", msg, tostring(args[i]))
	end
	logWarningFunc(msg)
end

function printError(...)
	if __LUA_LOG_LEVEL__ > LOG_LEVEL_ERROR then
		return
	end

	local args = {...}
	local count = #args
	if count <= 0 then return end

	--警告忽略__PRINT_TYPE__
	local msg = ""
	for i = 1, count do
		msg = string.format("%s%s ", msg, tostring(args[i]))
	end
	msg = msg .. getTraceback()
	logErrorFunc(msg)
end

--以lua语法来打印table
function printTable(...)
	if __LUA_LOG_LEVEL__ > LOG_LEVEL_WARNING then
		return
	end

	local args = {...}
	local fromIndex = 1
	if __PRINT_TYPE__ > 0 then
		if args[1] ~= __PRINT_TYPE__ then return end
		fromIndex = 2
	end
	if __PRINT_WITH_FILE_LINE__ then
		local traceback = string.split(debug.traceback("", 2), "\n")
		logFunc(string.format("print from: %s\n", string.trim(traceback[2])))
	end
	for i = fromIndex, #args do
		local root = args[i]
		if type(root) == "table" then
			local temp = {
				"----------------printTable start----------------------------\n",
				tostring(root).." = ",
			}
			table.insert(temp, table2String(root))
			table.insert(temp, "\n------------------------printTable end------------------------------")
			logFunc(table.concat(temp))
		else
			logFunc(tostring(root))
		end
	end
end

function table2BriefString(t, depth)
	if __LUA_LOG_LEVEL__ > LOG_LEVEL_WARNING then
		return ""
	end

	depth = depth or 0
	depth = depth + 1
	if depth > 3 then
		return "***"
	end
	local ret = "{"
	for k, v in pairs(t) do
		if type(k) == "string" then
			k = string.format("\"%s\"", k)
		end
		local vt = type(v)
		if vt == "function" or vt == "userdata" then
		elseif vt == "table" then
			ret = string.format("%s%s:%s,", ret, tostring(k), table2BriefString(v, depth))
		else
			ret = string.format("%s%s:%s,", ret, tostring(k), tostring(v))
		end
	end
	return string.format("%s}", ret)
end

-- dump打印
local function dump_value_(v)
    if type(v) == "string" then
        v = "\"" .. v .. "\""
    end
    return tostring(v)
end
local function dump_key_(v)
    if type(v) == "string" then
        v = "[\"" .. v .. "\"]"
    elseif type(v) == "number" then
        v = "[" .. v .. "]"
    end
    return tostring(v)
end
local function _dump(printType, value, desciption, nesting, notTrace)
	if __LUA_LOG_LEVEL__ > LOG_LEVEL_WARNING then
		return
	end

    if __PRINT_TYPE__ > 0 and printType ~= __PRINT_TYPE__ then
        return
    end

    if type(nesting) ~= "number" then nesting = 25 end

    local lookupTable = {}
    local result = {}

    if not notTrace then
        local traceback = string.split(debug.traceback("", 2), "\n")
        result[#result + 1 ] = "dump from: " .. string.trim(traceback[3] or "")
    end

    local function dump_(value, desciption, indent, nest, keylen)
        desciption = desciption or "<var>"
        local spc = ""
        if type(keylen) == "number" then
            spc = string.rep(" ", keylen - string.len(dump_value_(desciption)))
        end
        if type(value) ~= "table" then
            result[#result + 1 ] = string.format("%s%s%s = %s,", indent, dump_key_(desciption), spc, dump_value_(value))
        elseif lookupTable[value] then
            result[#result + 1 ] = string.format("%s%s%s = *REF*,", indent, dump_key_(desciption), spc)
        else
            lookupTable[value] = true
            if nest > nesting then
                result[#result + 1 ] = string.format("%s%s = *MAX NESTING*,", indent, dump_value_(desciption))
            else
                result[#result + 1 ] = string.format("%s%s = {", indent, dump_key_(desciption))
                local indent2 = indent.."    "
                local keys = {}
                local keylen = 0
                local values = {}

                for k, v in pairs(value) do
                    keys[#keys + 1] = k
                    local vk = dump_value_(k)
                    local vkl = string.len(vk)
                    if vkl > keylen then keylen = vkl end

                    --针对cdl结构做特殊处理，使打印的数据可读性更高
                    -- if value.__cname ~= nil and
                    --     (k == "__keyPrototype" or k == "__keyBase" or k == "__valuePrototype" or k == "__valueBase") then
                    --     local typeV = type(v)
                    --     if typeV ~= "table" then
                    --         values[k] = tostring(v)
                    --     else
                    --         values[k] = v.name
                    --     end
                    -- else
                    --     values[k] = v
                    -- end
                    values[k] = v
                end
                table.sort(keys, function(a, b)
                    if type(a) == "number" and type(b) == "number" then
                        return a < b
                    else
                        return tostring(a) < tostring(b)
                    end
                end)
                for i, k in ipairs(keys) do
                    dump_(values[k], k, indent2, nest + 1, keylen)
                end
                result[#result + 1] = string.format("%s}", indent)
            end
        end
    end
    dump_(value, desciption, "", 1)
    local ret = table.concat(result, "\n")
    return ret
end

function dump(...)
	local ret = _dump(...)
	if ret ~= nil then
	    logFunc(ret)
	end
end

function dumpString(...)
	local ret = _dump(...)
	return ret
end


--获取当前函数的调用堆栈
function getTraceback(fromLevel)
	if not __PRINT_TRACE_BACK__ then
		return ""
	end

	local ret = ""
	local level = 3
	if type(fromLevel) == "number" and fromLevel >= 0 then
		level = level + fromLevel
	end

	while true do
		local info = debug.getinfo(level, "Sln")
		if not info then
			break
		else
			if ret ~= "" then
				ret = ret .. "\r\n"
			end
			ret = string.format("%s[%s]:%d in %s \"%s\"", ret, info.source, info.currentline, info.namewhat ~= "" and info.namewhat or "''", info.name or "")
		end

		level = level + 1
	end

	ret = string.format("\n-------------------- Traceback --------------------\n%s\n", ret)

	return ret
end

--打印当前函数的调用堆栈
function printTraceback(fromLevel)
	if not __PRINT_TRACE_BACK__ then
		return
	end
	logFunc(getTraceback(fromLevel))
end

-- 打印函数的信息
function printStack(func, str)
	if not __PRINT_TRACK__ then
		return
	end

	if type(func) == "table" then
		return 
	end
	func = func or 2

	local t = {
		"[[",
		string.gsub(debug.getinfo(func).source, [[%.]], [[/]]),
		debug.getinfo(func).linedefined,
		debug.getinfo(func).lastlinedefined,
		"]]",
	}
	str = str or ""
	str = ""
	print(__PRINT_TYPE__, table.concat(t, " "), str)
end


local s_hookData = false
local millisecondNow = millisecondNow
function debugModuleBegin()
	if rawget(_G, "profiler_start") then
		--C++ 统计时间异步处理打印
		rawget(_G, "profiler_start")()
	else
		if s_hookData then
			return
		end

		s_hookData = {
			hookTime = 0,
			func = {},
			call = {},
			self = {},
			enter = {},
			duration = {},
			selfLast = {},
		}

		local count = 500
		local function hookf(type)
			if type == "count" then
				s_hookData.hookTime = s_hookData.hookTime + 1
				local info = debug.getinfo(2, "Sln")
				local name = string.format("%s[%d", info.source, info.linedefined)
				s_hookData.self[name] = (s_hookData.self[name] or 0) + 1
				-- s_hookData.enter[name] = millisecondNow()

			elseif type == "return" then
				local level = 2
				local line = 0
				local checkRepeat = {}
				local duration = 0
				while true do
					local info = debug.getinfo(level, "Sln")
					if not info then
						break
					else
						if info then
							if level == 2 then
								local key = string.format("%s[%d", info.source, info.linedefined)
								s_hookData.call[key] = (s_hookData.call[key] or 0) + 1
								line = (s_hookData.self[key] or 0) - (s_hookData.selfLast[key] or 0)
								s_hookData.selfLast[key] = s_hookData.self[key]
								-- if s_hookData.enter[key] then
								-- 	 duration = millisecondNow() - s_hookData.enter[key]
									-- s_hookData.duration[key] = (s_hookData.duration[key] or 0) +
								-- end
							end

							local name = string.format("%s[%d~%d]%s[%d]", info.source, info.linedefined, info.lastlinedefined, info.name, info.currentline)
							if not checkRepeat[name] then
								s_hookData.func[name] = (s_hookData.func[name] or 0) + line
								-- s_hookData.duration[name] = (s_hookData.duration[name] or 0) + duration
								checkRepeat[name] = true
							end
						end
					end
					level = level + 1
				end
			end
			-- count = count - 1
			-- if count < 0 then
			-- 	debug.sethook()
			-- end
		end
		logFunc("======== debugModuleBegin ========")
		debug.sethook(hookf, "r", 1)
	end
end

function debugModuleEnd()
	if rawget(_G, "profiler_stop") then
		rawget(_G, "profiler_stop")()
	else
		debug.sethook()
		if not s_hookData then
			return
		end
		logFunc("======== debugModuleEnd ========")
		logFunc("== total hooks:" .. s_hookData.hookTime .. "==")

		local t = {}
		for k, v in pairs(s_hookData.func) do
			local call = 1
			local pos = string.find(k, "~")
			if pos then
				local ck = string.sub(k, 1, pos - 1)
				call = s_hookData.call[ck] or 1
			end
			-- local duration = s_hookData.duration[k]
			table.insert(t, {
				name = k,
				hooks = v,
				-- duration = duration,
				calls = call,
				hooksPreCall = math.floor(v / call),
			})
		end
		table.sort(t, function(a, b)
			return a.hooks > b.hooks
		end)
		logFunc("|  hooks  |hooks pre call|  call   |  duration  |   location   |")
		for i = 1, 90 do
			local v = t[i]
			if not v then
				break
			end
			-- logFunc(string.format("%10d %10d %10d  %14f   %s", v.hooks, v.hooksPreCall, v.calls, v.duration, v.name))
			logFunc(string.format("%10d %10d %10d  %s", v.hooks, v.hooksPreCall, v.calls, v.name))
		end
		s_hookData = false
	end
end
