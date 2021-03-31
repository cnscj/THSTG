---@class TimeUtil
local M = {}

local socket = require "socket"
local MathToInteger = math.tointeger
local math = math

local pattern_full, sz_full = "(%d+)%-(%d+)%-(%d+)[T ](%d+):(%d+):(%d+)%.(%d+)", 23
local pattern_sec, sz_sec = "(%d+)%-(%d+)%-(%d+)[T ](%d+):(%d+):(%d+)", 19
local pattern_day, sz_day = "(%d+)%-(%d+)%-(%d+)", 10

local format_normal_date = "%Y-%m-%d"

local perday = 24 * 60 * 60
local msperday = 24 * 60 * 60 * 1000
local msperhour = 60 * 60 * 1000
local msperminute = 60 * 1000
local mspersecond = 1000


function M.getTimeStamp()
    return os.time()
end

function M.getTimeStampMs()
    local start_time = socket.gettime()
    local end_time= socket.gettime()
    local use_time = (end_time - start_time )*1000
    return use_time
end

-- params = {
-- 	year = 2017,--必须包含字段
-- 	month = 10,	--必须包含字段
-- 	day = 26,	--必须包含字段
-- 	hour = 14,	--默认值0
-- 	min = 55,	--默认值0
-- 	sec = 10, 	--默认值0
-- 	isdst = nil --默认值nil
-- } 或者 params = nil
function M.getTime(params)
	return os.time(params)
end

function M.getWhatDayByTimeStamp(timeStamp)
    return tonumber(os.date("%w", timeStamp))
end

-- 年月日计算星期几(周一到周六，1~6，周日是0)
function M.getWhatDay(y, m, d)
    --在蔡勒公式中，某年的1、2月要看作上一年的13、14月来计算
    if m == 1 then
        y = y - 1
        m = 13
    end
    if m == 2 then
        y = y - 1
        m = 14
    end
    local c = math.floor(y / 100)
    y = y - c * 100
    local w = y + math.floor(y / 4) + math.floor(c / 4) - 2 * c + math.floor(26 * (m + 1) / 10) + d - 1
    return w % 7
end

---@return boolean@是否同一周
function M.isBetweenOneWeek(time1, time2)
    local diff = time1 - time2
    if math.abs(diff) > ConstVars.ONE_WEEK then
        return false
    end
    if time1 == time2 then
        return true
    end
    --保证 time1 > time2
    if diff < 0 then
        time1, time2 = time2, time1
    end
    for i = 0, 6 do
        local nextDay = time2 + i * ConstVars.ONE_DAY
        if nextDay > time1 then
            return true
        end
        -- 跨过了星期天
        if M.getWhatDayByTimeStamp(nextDay) == 0 then
            return false
        end
    end
    return false
end

-- xx:xx:xx的格式化
function M.formatTime(seconds)
    seconds = math.max(0, seconds)
    local hour = math.floor(seconds / 3600)
    hour = hour < 10 and ("0" .. hour) or hour
    local min = math.floor(seconds % 3600 / 60)
    min = min < 10 and ("0" .. min) or min
    local sec = math.floor(seconds % 60)
    sec = sec < 10 and ("0" .. sec) or sec

    return hour .. ":" .. min .. ":" .. sec
end

--秒转化为时间,天，小时， 分
function M.getTime(second)
    --秒
    local v = tonumber(second)
    local day, hour, min, sec = 0, 0, 0, 0
    if v and v >= 0 then
        day = math.floor(v / 86400)
        hour = math.floor((second - day * 86400) / 3600)
        min = math.floor((second - day * 86400 - hour * 3600) / 60)
        sec = second - day * 86400 - hour * 3600 - min * 60
    end
    return day, hour, min, sec
end

--秒转化为 时-分
function M.getHourTime(second)
    --秒
    local v = tonumber(second)
    local hour, min = 0, 0
    if v and v >= 0 then
        hour = math.floor(v / 3600)
        min = math.floor((v - hour * 3600) / 60)
    end
    return hour, min
end

-- 剩余时间 转换 天-时-分-秒
function M.getChineseTime(value, space, ignoreMin, ignoreSec)
    local day, hour, min, sec = M.getTime(value)
    space = space or string.empty

    local str = ""
    if day > 0 then
        str = str .. day .. Language.getString(100110) .. space
    end
    if hour > 0 then
        str = str .. hour .. Language.getString(100111) .. space
    end
    -- 省略秒的时候,最低一分钟
    if ignoreSec and min < 1 and sec >= 1 then
        min = 1
    end
    if not ignoreMin and min >= 1 then
        str = str .. min .. Language.getString(100112) .. space
    end
    if not ignoreSec and sec >= 1 then
        str = str .. sec .. Language.getString(100113)
    end
    return str
end

function M.transWarehouseTime(value)
    -- 显示 d-h-m
    local day, hour, min, sec = M.getTime(value)

    if day > 3 then
        return string.format("%dday", day)
    elseif day > 0 then
        return string.format("%dh", hour + day * 24)
    elseif hour > 0 then
        return string.format("%dh", hour)
    elseif min > 0 then
        return string.format("%dmin", min + 1)
    else
        return Language.getString(101006)
    end
    return ""
end

function M.transTime(time)
    local day = math.floor(time / 3600 / 24)
    local hour = math.floor(time / 3600 % 24)
    local min = math.floor(time / 60 % 60)
    local sec = math.floor(time % 60)
    return day, hour, min, sec
end

function M.transTimeWithoutDay(time)
    local hour = math.floor(time / 3600)
    local min = math.floor(time / 60 % 60)
    local sec = math.floor(time % 60)
    return hour, min, sec
end

function M.transTimeText(time, ignoreSec)
    local a, b, c, d = M.transTime(time)
    local aText = ""
    local bText = ""
    local cText = ""
    local dText = ""
    if a > 0 then
        aText = a .. Language.getString(105000)
    end
    if b > 0 then
        bText = b .. Language.getString(105009)
    end
    if c > 0 then
        cText = c .. Language.getString(105002)
    end
    if d > 0 then
        if ignoreSec and a <= 0 then
            dText = ""
        else
            dText = d .. Language.getString(105003)
        end
    end
    return string.format("%s%s%s%s", aText, bText, cText, dText)
end

--- 时分秒
function M.transTimeTextWithoutDay(time, ignoreSec)
    local b, c, d = M.transTimeWithoutDay(time)
    local aText = ""
    local bText = ""
    local cText = ""
    local dText = ""
    if b > 0 then
        bText = b .. Language.getString(105009)
    end
    if c > 0 then
        cText = c .. Language.getString(105002)
    end
    if d > 0 then
        if ignoreSec and a <= 0 then
            dText = ""
        else
            dText = d .. Language.getString(105003)
        end
    end
    return string.format("%s%s%s%s", aText, bText, cText, dText)
end

function M.transTimeShortText(time)
    local a, b, c, d = M.transTime(time)
    local aText = ""
    local bText = ""
    local cText = ""
    local dText = ""
    if a > 0 and b > 0 then
        aText = a .. Language.getString(105000)
        bText = b .. Language.getString(105009)
    else
        cText = c .. Language.getString(105002)
        dText = d .. Language.getString(105003)
    end

    return string.format("%s%s%s%s", aText, bText, cText, dText)
end

---09:00:00转换成多少秒
function M.transTimeStrToSeconds(str)
    local tb = string.split(str, ":")
    local h = tonumber(tb[1])
    local m = tonumber(tb[2])
    local s = #tb < 3 and 0 or tonumber(tb[3])
    return h * 3600 + m * 60 + s
end

--"2019-06-27 19:48:57"
function M.transTimeStrToTimestamp(str, timeRegular)
    local strDate = str
    timeRegular = timeRegular or "(%d+)-(%d+)-(%d+)%s*(%d+):(%d+):(%d+)"
    local _, _, y, m, d, _hour, _min, _sec = string.find(strDate, timeRegular)
    --转化为时间戳
    local timestamp = os.time({ year = y, month = m, day = d, hour = _hour, min = _min, sec = _sec })
    return timestamp
end

--"2019-06-27 19:48:57"
function M.transTimeStr2TimeTb(str)
    local start = string.gmatch(str, "%d+")
    local y = start()
    local m = start()
    local d = start()
    local hour = start()
    local min = start()
    local sec = start()
    return {
        year = y,
        month = m,
        day = d,
        hour = hour,
        min = min,
        sec = sec
    }
end
--

-- 格式化时间：xx秒，xx分钟，xx小时，xx天
function M.getFormatTimeByType(seconds, type)
	local value = getOffsetTime(seconds)
	if type == "s" then
		return value
	elseif type == "m" then
		return math.floor(value / 60)
	elseif type == "h" then
		return math.floor(value / 3600)
	elseif type == "d" then
		return math.floor(value / 86400)
	end
end

-- xx:xx:xx的格式化
function M.formatTime(seconds, showHour, hideSec)
	local hour = math.floor(seconds / 3600)
	hour = hour < 10 and ("0"..hour) or hour
	local min = math.floor(seconds%3600 / 60)
	min = min < 10 and ("0"..min)or min
	local sec = math.floor(seconds%60)
	sec = sec < 10 and ("0"..sec)or sec

	if showHour or seconds >= 3600 then
		return hour..":"..min .. ( not hideSec and ( ":" .. sec ) or "")
	else
		return min..":"..sec
	end
end


function M.getYear(ms)
	ms = ms or M.getTimeStampMs()
	return MathToInteger(os.date("%Y", math.floor(ms / 1000)))
end

function M.getMonth(ms)
	ms = ms or M.getTimeStampMs()
	return MathToInteger(os.date("%m", math.floor(ms / 1000)))
end

function M.getDay(ms)
	ms = ms or M.getTimeStampMs()
	return MathToInteger(os.date("%d", math.floor(ms / 1000)))
end

function M.getHour(ms)
	ms = ms or M.getTimeStampMs()
	return MathToInteger(os.date("%H", math.floor(ms / 1000)))
end

function M.getMin(ms)
	ms = ms or M.getTimeStampMs()
	return MathToInteger(os.date("%M", math.floor(ms / 1000)))
end

function M.getSec(ms)
	ms = ms or M.getTimeStampMs()
	return MathToInteger(os.date("%S", math.floor(ms / 1000)))
end

function M.getHNM(ms)
	ms = ms or M.getTimeStampMs()
	return os.date("%H:%M", math.floor(ms / 1000))
end

function M.getHMS(ms)
	ms = ms or M.getTimeStampMs()
	return os.date("%H:%M:%S", math.floor(ms / 1000))
end

function M.getYMDHMS(ms)
	ms = ms or M.getTimeStampMs()
	return os.date("%Y-%m-%d %H:%M:%S", math.floor(ms / 1000))
end

function M.getTotalDays(ms)
	ms = ms or M.getTimeStampMs()
	local timeZoneDiffMs = Cache.serverTimeCache:getCurTimeZone() * msperhour
	return math.floor((ms-timeZoneDiffMs) / msperday) + 1
end

local WeekIndex = {
	Monday = 1 ,
	Tuesday = 2 ,
	Wednesday = 3 ,
	Thursday = 4 ,
	Friday = 5 ,
	Saturday = 6 ,
	Sunday = 7 ,
}
function M.getWeekDay2(ms)
	ms = ms or M.getTimeStampMs()
	local fixedTime = math.floor(ms / 1000)
	local name = os.date("%A", fixedTime)

	return WeekIndex[name] or 0
end

function M.getDiffDay(ms1, ms2)
	-- print
	return M.getTotalDays(ms1) - M.getTotalDays(ms2)
end


--获取下一个24时的时间戳，即curTime当天的24时
--curTime: 当前时间戳，秒级
function M.getNextTimestampOf24(curTime)
    local timeTb = os.date("*t", curTime)
    --为了方便，这边计算当天23:59:59的时间戳，因为再多1秒，可能就跨月跨年了，年月日传参会有点麻烦。
    local toTime = os.time({year = timeTb.year, month = timeTb.month, day = timeTb.day, hour = 23, min = 59, sec = 59})
    --再加1秒
    toTime = toTime + 1
    return toTime
end


--获取到第二日凌晨的剩余时间 返回秒数
function M.getRemainSecondsTo24()
	local curMs = M.getTimeStampMs()
	local toYear = TimeLib.getYear(curMs)
	local toMonth = TimeLib.getMonth(curMs)
	local toDay = TimeLib.getDay(curMs)
	local toTime = os.time({year =toYear, month = toMonth, day =toDay, hour =23, min =59, sec = 59})
	return toTime - math.ceil(curMs/1000) + 1;   --为什么+1？因为我们返回的是当日23：59：59的秒数，如果是第二天凌晨的话就需要多加1
end

--获取到下周一凌晨的剩余时间 返回秒数
function M.getRemainSecondsToNextWeek()
	return M.getRemainSecondsTo24() +  (7 - M.getWeekDay2())*24*60*60
end

--获取到下月一号凌晨的剩余时间 返回秒数
function M.getRemainSecondsToNextMonth()
	local curMs = M.getTimeStampMs()
	local toYear = TimeLib.getYear(curMs)
	local toMonth = TimeLib.getMonth(curMs)
	local toDay = TimeLib.getDay(curMs)

	local needDay = 0
	if toMonth == 1 or toMonth == 3 or toMonth == 5 or toMonth == 7 or toMonth == 8 or toMonth == 10 or toMonth == 12 then
		needDay = 31
	elseif toMonth == 4 or toMonth == 6 or toMonth == 9 or toMonth == 11 then
		needDay = 30
	elseif toYear % 400 == 0 or toYear % 4 == 0 and toYear % 100 ~= 0 then
		needDay = 29
	else
		needDay = 28
	end

	local toTime = os.time({year =toYear, month = toMonth, day = needDay, hour =23, min =59, sec = 59})
	return toTime - math.ceil(curMs/1000) + 1;   --因为我们返回的是当月最后一天23：59：59的秒数，如果是第二天凌晨的话就需要多加1
end



local format_normal = "%Y-%m-%d %H:%M:%S"
function M.msToString(ms, format)
	format = format or format_normal
	return os.date(format, math.floor(ms / 1000))
end

-- xx天xx小时xx分xx秒
function M.getTimeFormatDay(less_time, isIgnoreZero,color)
	less_time = tonumber(less_time) or 0
	local day = math.floor(less_time / 86400)
	local lessT = math.floor(less_time%86400)
	local hour = math.floor(lessT / 3600)
	local min = math.floor((lessT % 3600) / 60)
	local sec = math.floor(lessT % 3600 % 60)
	local dayStr = ""
	
	if not isIgnoreZero then
		dayStr = day..Language.getString(105000)
		return dayStr..hour..Language.getString(105001)..min..Language.getString(105002)..sec..Language.getString(105003)
	else
		local isZeroAdd = false -- 为0的时候是否添加
		if day > 0 then
			if color then
				day = string.format("[color=%s]%s[/color]",color,day)
			end
			dayStr = dayStr .. day .. Language.getString(105000)
		else
			isZeroAdd = true
		end
		if isZeroAdd or hour > 0 then
			if color then
                hour = string.format("[color=%s]%s[/color]",color,hour)
            end
			dayStr = dayStr .. hour .. Language.getString(105001)
		else
			isZeroAdd = true
		end
		if isZeroAdd or min > 0 then
            if color then
                min = string.format("[color=%s]%s[/color]",color,min)
            end
            dayStr = dayStr .. min .. Language.getString(105002)
		else
			isZeroAdd = true
		end
		if isZeroAdd or sec > 0 then
            if color then
                sec = string.format("[color=%s]%s[/color]",color,sec)
            end
            dayStr = dayStr .. sec .. Language.getString(105003)
		else
			isZeroAdd = true
		end
		return dayStr
	end
end

---获得xx:xx:xx的总秒数
function M.getAllSecTime(time)
	local timeArr = string.split(time,":")
	return tonumber(timeArr[1])*3600+tonumber(timeArr[2])*60+tonumber(timeArr[3])
end

function M.getMinSecTime(less_time)
	less_time = tonumber(less_time) or 0
	local hour = math.floor(less_time / 3600)
	hour = (hour < 10) and "0"..hour or hour
	local min = math.floor((less_time % 3600) / 60)
	min = (min < 10) and "0"..min or min
	local sec = less_time % 3600 % 60
	sec = (sec < 10) and "0"..sec or sec
	return  min .. ":" .. sec
end

function M.getMDHMS(less_time)
	return os.date("%m-%d %X ", less_time)
end

function M.getYMD(ms)
	return os.date(format_normal_date, math.floor(ms / 1000))
end

function M.ToTimestampEnterGame(dt)
	local xyear, xmonth, xday
	local xhour, xminute, xseconds, xmillisec = 0, 0, 0, 0
	local sz = #dt
	if sz <= sz_day then
		xyear, xmonth, xday = dt:match(pattern_day)
	elseif sz <= sz_sec then
		xyear, xmonth, xday, xhour, xminute, xseconds = dt:match(pattern_sec)
	else
		xyear, xmonth, xday, xhour, xminute, xseconds, xmillisec = dt:match(pattern_full)
	end
	if xyear == nil then
		return
	end
	return os.time({year = xyear, month = xmonth, day = xday, hour = xhour, min = xminute, sec = xseconds}) * 1000 + xmillisec
end


--[[
计算两个日期相差的天数，注意，只要过了凌晨就算相隔一天，无论是否超过24小时
@fromMs:起始时间戳 毫秒级
@toMs:结束时间戳 毫秒级
@return 两个日期相隔的天数
]]
function M.DifDate(fromMs, toMs)
	local fromhourstamp = fromMs - GetDateStamp(fromMs)
	local tohourstamp = toMs - GetDateStamp(toMs)
	local days = M.DifDay(fromMs, toMs)
	if fromhourstamp > tohourstamp then
		days = days + 1
	end
	return days
end

--[[
计算两个日期相差的天数，注意，这个不同于DifDate，就是计算实际的时间差
@fromMs:起始时间戳 毫秒级
@toMs:结束时间戳 毫秒级
@return 两个时间点相隔的天数
]]
function M.difDay(fromMs, toMs)
	local diff = toMs - fromMs
	return math.floor(diff / msperday)
end

---返回一天共有多少秒
function M.getOneDaySeconds()
	return perday
end

-- <60min   XXX（取整）分钟前
-- >60分钟   XXX（取整）小时前
-- >24小时   X（取整）天前
function M.getRecordTime(ms)
	ms = ms or M.getTimeStampMs()
	local day = math.floor(ms / msperday)
	if day > 0 then
		return Language.getString(484326,day)
	end
	local hour = math.floor(ms / msperhour)
	if hour > 0 then
		return Language.getString(484327, hour)
	end
	local min = math.ceil(ms / msperminute)
	return Language.getString(484328, min)
end

rawset(_G, "TimeTool", M)
