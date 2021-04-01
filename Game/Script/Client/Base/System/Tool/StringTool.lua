---@class StringUtil
local M = {}

--[[
字符串相关
]]


function M.filterString(s)
    --当输入的数字连续有8个以上，也按照屏蔽词来处理，发出去后变为**
    local str = CSharp.FilterEncryptUtil.FilterString(s)
    local isMatch = string.match(str,"%d%d%d%d%d%d%d%d")
    if isMatch then
        -- str = string.gsub(str,isMatch,"********")
        str = M.findConsecutiveNum(str)
    end
    return str
end

function M.findConsecutiveNum(str)
    local index = 0
    local target = str
    for word in string.gmatch(str,"%d+") do
        if string.len(word) >= 8 then
            target = string.gsub(target,word,"********")
        end
    end

    return target
end

function M.containsInvalidChar(s)
    return CSharp.FilterEncryptUtil.ContainsInvalidChar(s)
end

-- keepRadix 是否保留小数点
function M.numToString(num, keepRadix, radixNum)
    if not num or type(num) ~= 'number' then
        return ""
    end
    if keepRadix then
        local ext = ""
        radixNum = radixNum or 2
        local radix = (1 / math.pow(10, radixNum))
        if num >= 100000000 then
            num = num / 100000000
            num = num - num % radix
            ext = Language.getString(100014)--亿
        elseif num >= 10000 then
            num = num / 10000
            num = num - num % radix
            ext = Language.getString(100013)--万
        end
        return num .. ext
    else
        local showText = tostring(num)
        if string.len(showText) >= 9 then
            showText = string.sub(showText, 1, -9)
            showText = showText .. Language.getString(100014)
        elseif string.len(showText) >= 5 then
            showText = string.sub(showText, 1, -5)
            showText = showText .. Language.getString(100013)
        end
        return showText
    end
end

--[[
    获取数字对应中文字符串(目前可以转换0 ~ 99999)
    @example    getChineseNumString(10034) return  "一万零三十四"
--]]
function M.getChineseNumString(number)
    if type(number) ~= 'number' or number < 0 then
        local value = tostring(number)
        printWarning("getChineseNumString()入参非数字或者<0！", value)
        return value
    end

    local zero = 100000
    
    if number == 0 then
        return Language.getString(zero)
    end

    local ten = 100010
    local result = ""

    local i = 1             --数位记录
    local pre0 = false      --上一位是否是0
    local temp0 = false     --缓存字符零
    local temp1 = false     --缓存字符一
    while number ~= 0 do
        local n = number % 10    --当前位数字
        --个位
        if i == 1 then
            if n ~= 0 then
                result = Language.getString(zero + n)
            else
                pre0 = true
            end
            --十位
        elseif i == 2 then
            if n == 0 then
                if not pre0 then
                    temp0 = Language.getString(zero)
                end
                pre0 = true
            elseif n == 1 then
                temp1 = Language.getString(zero + 1)
                result = string.format("%s%s", Language.getString(ten), result)
                pre0 = false
            else
                result = string.format("%s%s%s", Language.getString(zero + n), Language.getString(ten), result)
                pre0 = false
            end
            --其他位
        else
            if n == 0 then
                if not pre0 then
                    temp0 = Language.getString(zero)
                end
                if temp1 then
                    result = string.format("%s%s", Language.getString(zero + 1), result)
                    temp1 = false
                end
                pre0 = true
            else
                if temp0 then
                    result = string.format("%s%s", Language.getString(zero), result)
                    temp0 = false
                end
                if temp1 then
                    result = string.format("%s%s", Language.getString(zero + 1), result)
                    temp1 = false
                end
                result = string.format("%s%s%s", Language.getString(zero + n), Language.getString(ten + i - 2), result)
                pre0 = false
            end
        end
        number = math.floor(number / 10)
        i = i + 1
    end
    return result
end

--格式化大数值  >=1 万  100000000 亿
---@param value number 要转化的数值
---@param wan number 万位
---@param wanDescId number 万位描述ID
---@param yi number 亿位
---@param yiDescId number 亿位描述ID
---@param wanYi number 万亿位
---@param wanYiDescId number 万亿位描述ID
function M.formatLargeNumToSting(value,wan,wanDescId,yi,yiDescId,wanYi,wanYiDescId)
    if type(value) ~= "number" then
        return
    end
    local result = 0
    local strResult = value
    wan = wan or 10000
    yi = yi or 100000000
    wanYi = wanYi or 1000000000000
    wanDescId = wanDescId or 100015
    yiDescId = yiDescId or 100016
    wanYiDescId = wanYiDescId or 100017
    if value >= wanYi then
        result = math.floor((value / wanYi * 100)) * 0.01 -- %.2f万亿
        strResult = Language.getString(wanYiDescId, result)
    elseif value >= yi then
        result = math.floor((value / yi * 100)) * 0.01 -- %.2f亿
        strResult = Language.getString(yiDescId, result)
    elseif value >= wan then
        result = math.floor((value / wan * 100)) * 0.01 -- %.2f万
        strResult = Language.getString(wanDescId, result)
    end
    return strResult
end

--格式化大数值  >=10000 万  100000000 亿
--function M.formatLargeNumToSting(value)
--    if type(value) ~= "number" then
--        return
--    end
--    local result = 0
--    local strResult = value
--    if value >= 1000000000000 then
--        result = math.floor((value / 1000000000000 * 100)) * 0.01 -- %.2f万亿
--        strResult = Language.getString(100017, result)
--    elseif value >= 100000000 then
--        result = math.floor((value / 100000000 * 100)) * 0.01 -- %.2f亿
--        strResult = Language.getString(100016, result)
--    elseif value >= 10000 then
--        result = math.floor((value / 10000 * 100)) * 0.01 -- %.2f万
--        strResult = Language.getString(100015, result)
--    end
--    return strResult
--end

--格式化大数值  >=10000 万  1000000000(10亿才显示) 亿
function M.formatLargeNumToSting2(value)
    if type(value) ~= "number" then
        return
    end
    local result = 0
    local strResult = value
    if value >= 1000000000000 then
        result = math.floor((value / 1000000000000 * 100)) * 0.01 -- %.2f万亿
        strResult = Language.getString(100017, result)
    elseif value >= 1000000000 then
        result = math.floor((value / 100000000 * 100)) * 0.01 -- %.2f十亿
        strResult = Language.getString(100016, result)
    elseif value >= 10000 then
        result = math.floor((value / 10000 * 100)) * 0.01 -- %.2f万
        strResult = Language.getString(100015, result)
    end
    return strResult
end

--向下取整余数不为0显示1位小数
function M.formatLargeNumToSting3(value)
    if type(value) ~= "number" then
        return
    end
    local result = 0
    local strResult = value
    if value >= 1000000000000 then
        result = math.floor((value / 1000000000000 * 100)) * 0.01 -- %.2f万亿
        local _,rem = math.modf(result)
        local formatStr = (rem == 0) and 100020 or 100023
        strResult = Language.getString(100023, result)
    elseif value >= 1000000000 then
        result = math.floor((value / 100000000 * 100)) * 0.01 -- %.2f十亿
        local _,rem = math.modf(result)
        local formatStr = (rem == 0) and 100019 or 100022
        strResult = Language.getString(100022, result)
    elseif value >= 10000 then
        result = math.floor((value / 10000 * 100)) * 0.01 -- %.2f万
        local _,rem = math.modf(result)
        local formatStr = (rem == 0) and 100018 or 100021
        strResult = Language.getString(formatStr, result)
    end
    return strResult
end

--[[将str的第一个字符转化为大写字符。成功返回转换后的字符串，失败返回nil和失败信息]]
function M.capitalize(str)
    if str == nil then
        return nil, "the string parameter is nil"
    end
    local ch = string.sub(str, 1, 1)
    local len = string.len(str)
    if ch < 'a' or ch > 'z' then
        return str
    end
    ch = string.char(string.byte(ch) - 32)
    if len == 1 then
        return ch
    else
        return ch .. string.sub(str, 2, len)
    end
end

--[[将str的第一个字符转化为小写字符。成功返回转换后的字符串，失败返回nil和失败信息]]
function M.lowercase(str)
    if str == nil then
        return nil, "the string parameter is nil"
    end
    local ch = string.sub(str, 1, 1)
    local len = string.len(str)
    if ch < 'A' or ch > 'Z' then
        return str
    end
    ch = string.char(string.byte(ch) + 32)
    if len == 1 then
        return ch
    else
        return ch .. string.sub(str, 2, len)
    end
end

local _chnNum
local function getChnNumTb()
    if not _chnNum then
        _chnNum = {
            [0] = Language.getString(100000),
            [1] = Language.getString(100001),
            [2] = Language.getString(100002),
            [3] = Language.getString(100003),
            [4] = Language.getString(100004),
            [5] = Language.getString(100005),
            [6] = Language.getString(100006),
            [7] = Language.getString(100007),
            [8] = Language.getString(100008),
            [9] = Language.getString(100009),
            [10] = Language.getString(100010),
            [100] = Language.getString(100011),
        }
    end
    return _chnNum
end

function M.formatMemory(bitSize, withSuffix)
    local kSize = bitSize / 1024
    if kSize < 1 then
        return string.format("%s%s", tostring(bitSize), (withSuffix and "B" or ""))
    end

    local mSize = kSize / 1024
    if mSize < 1 then
        return string.format("%0.2f%s", kSize, (withSuffix and "K" or ""))
    end

    local gSize = mSize / 1024
    if gSize < 1 then
        return string.format("%0.2f%s", mSize, (withSuffix and "M" or ""))
    end

    return string.format("%0.2f%s", gSize, (withSuffix and "G" or ""))
end

function M.formatMemoryProgress(curBitSize, totalBitSize)
    local kSize0 = curBitSize / 1024
    local kSize1 = totalBitSize / 1024
    if kSize1 < 1 then
        return string.format("%sB/%sB", tostring(curBitSize), tostring(totalBitSize))
    end

    local mSize0 = kSize0 / 1024
    local mSize1 = kSize1 / 1024
    if mSize1 < 1 then
        return string.format("%0.2fK/%0.2fK", kSize0, kSize1)
    end

    local gSize0 = mSize0 / 1024
    local gSize1 = mSize1 / 1024
    if gSize1 < 1 then
        return string.format("%0.2fM/%0.2fM", mSize0, mSize1)
    end

    return string.format("%0.2fG/%0.2fG", gSize0, gSize1)
end


--[[值的转换， 值达到100万时，显示为100万；当值达到10亿时，显示为10亿
如果firstMarkValue有值，则以firstMarkValue为准
showDecimal 小数
decimalNum 小数位数
]]
function M.getCoinValue(value, firstMarkValue, showDecimal, decimalNum)
    local head = tonumber(value)
    if not head then
        return "0"
    end
    
    if firstMarkValue and firstMarkValue > value then
        return value
    end
    local tail = ""
    local firstMax = firstMarkValue or 1000000
    local secondMax = 1000000000
    local firstDev = math.ceil(firstMax / 10000)
    if head >= firstMax and head < secondMax then
        tail = Language.getString(100013)
        if showDecimal then
            head = head / (firstMax / firstDev)
            if type(decimalNum) == "number" then
                head = string.format("%." .. decimalNum .. "f", head)
            end
        else
            head = math.floor(head / (firstMax / firstDev))
        end
    elseif head >= secondMax then
        tail = Language.getString(100014)
        if showDecimal then
            head = head / (secondMax / 10)
            if type(decimalNum) == "number" then
                head = string.format("%." .. decimalNum .. "f", head)
            end
        else
            head = math.floor(head / (secondMax / 10))
        end
    end
    return tostring(head) .. tostring(tail)
end

-- 字符串 转 字符串
-- 阿拉伯0-99 转换 中文
function M.transNumToChnNum(num)
    if num == nil then
        return ""
    end
    if type(num) == "string" then
        num = tonumber(num)
    end

    local chnNum = getChnNumTb()

    if num >= 0 and num <= 10 then
        return chnNum[num]
    elseif num > 10 and num <= 19 then
        local ge = num % 10
        return chnNum[10] .. chnNum[ge]
    elseif num >= 20 and num < 100 then
        local shi = math.floor(num / 10)
        local shiStr = chnNum[shi]

        local ge = num % 10
        local geStr = ""
        if ge > 0 then
            geStr = chnNum[ge]
        end
        return shiStr .. chnNum[10] .. geStr
    elseif num >= 100 and num < 1000 then
        local bai = math.floor(num / 100)
        local baiStr = chnNum[bai] .. chnNum[100]
        local sub = num % 100
        if sub == 0 then
            return baiStr
        end
        local subStr = ""
        if sub < 20 then
            local shi = math.floor(sub / 10) % 10
            subStr = chnNum[shi]
        end
        return baiStr .. subStr .. transNumToChnNum(sub)
    end
    return ""
end

function M.transWeekDayToChinese(num)
    return GameDefConfig.getWeekday(num)
end

function M.transWeekDayToChineseTwo(num)
    return GameDefConfig.getWeekdayTwo(tonumber(num))
end


-- 时间 => m:s.ms
function M.transTimeMS(MS,ignoreMs,modM)
    local min = math.floor(MS / 1000 / 60)
    if modM ~= true then min = math.floor(min % 60) end 

    local sec = math.floor(MS / 1000 % 60)
    local ms = math.floor(MS % 1000 / 10)
    if ignoreMs then
        return string.format("%02d:%02d", min, sec)
    else
        return string.format("%02d:%02d.%02d", min, sec, ms)
    end
end

-- 时间 => h:m:s
function M.transTimeStr(time, isShowHour)
    if time < 0 then
        if isShowHour then
            return string.format("%02d:%02d:%02d", 0, 0, 0)
        else
            return string.format("%02d:%02d", 0, 0)
        end
    end
    local hour = math.floor(time / 3600)
    local min = math.floor(time / 60 % 60)
    local ms = math.floor(time % 60)
    if isShowHour or hour > 0 then
        return string.format("%02d:%02d:%02d", hour, min, ms)
    else
        return string.format("%02d:%02d", min, ms)
    end
end

function M.transExp(exp)
    local res = exp
    if exp >= 1000000000000 then
        res = exp / 1000000000000
        res = Language.getString(100017, res)
    elseif exp >= 100000000 then
        res = exp / 100000000
        res = Language.getString(100016, res)
    elseif exp >= 10000 then
        res = exp / 10000
        res = Language.getString(100015, res)
    end
    return tostring(res)
end

function M.transBigExp(exp)
    local res = math.floor(exp)
    if exp >= 1000000000000 then
        res = exp / 1000000000000
        res = Language.getString(100017, res)
    elseif exp >= 100000000 then
        res = exp / 100000000
        res = Language.getString(100016, res)
    end
    return tostring(res)
end

function M.transBigZDL(value)
    if value == nil then
        return ""
    end
    return M.transHP(value)
end

function M.transHP(value)
    if value >= 1 then
        if value >= 100000000 then
            value = string.format("%.2f%s", math.floor(value / 100000000 * 100) / 100, Language.getString(100014))
        elseif value >= 100000 then
            value = string.format("%.2f%s",  math.floor(value / 10000 * 100) / 100, Language.getString(100013))
        else
            value = string.format("%d", value)
        end
    else
        value = 0
    end
    return tostring(value)
end

function M.transBigInt(value)
    if value >= 1 then
        if value >= 10000000000 then
            value = string.format("%.0f%s", math.floor(value / 10000000000 * 100) / 100, Language.getString(100027))
        elseif value >= 100000000 then
            value = string.format("%.2f%s", math.floor(value / 100000000 * 100) / 100, Language.getString(100014))
        elseif value >= 10000 then
            value = string.format("%.0f%s",  math.floor(value / 10000 * 100) / 100, Language.getString(100013))
        else
            value = string.format("%d", value)
        end
    else
        value = 0
    end
    return tostring(value)
end


--解析像456,656#556,45 的字符串成table
--[[
 str 		原字符串
 splitTb 	分隔符列表 
 fields		生成字段名列表
--
 ex:
  conditTb = stringParser(condit,{"#",","},{
					{name = "equipType",isString = true},
					{name = "color"}
				})
]]--
function M.stringSplitParser(str, splitTb, fields)
    local result = {}
    local function loop(str, index)
        local splitWord = splitTb[index]
        if splitWord == "" then
            if fields[1].isString then
                return str
            else
                return tonumber(str)
            end
        end
        local tb = string.split(str, splitWord)
        local result = {}
        if index + 1 <= #splitTb then
            for i, v in ipairs(tb) do
                if v ~= "" then
                    table.insert(result, loop(v, index + 1))
                end
            end
        else
            local fieldCount = math.min(#fields, #tb)
            for i = 1, fieldCount do
                local isString = fields[i].isString
                local fieldName = fields[i].name
                local value = tb[i]
                if not isString then
                    value = tonumber(value)
                end
                result[fieldName] = value
            end
        end
        return result
    end
    return loop(str, 1)
end
---获得10/20的颜色字符串    
---@ param have number @拥有的，
---@ param need number @需要的,
---@ param showHave boolean @是否显示拥有的(默认显示）
function M.getColorAmountText(have, need, showHave)
    local str = ""
    showHave = showHave == nil or showHave
    if showHave then
        local desCode = 212090
        if have >= need then
            desCode = 212089
        end
        str = Language.getString(desCode, have, need)
    else
        if have >= need then
            str = Language.getString(213013, need)
        else
            str = Language.getString(213012, need)
        end
    end
    return str
end

--- 获取字符串中有多少个word(可以找%s和%%)
function M.findWord(str, word)
    if not str then
        return 0
    end
    local amount, init = 0, 0
    -- 防止死循环?
    local i = 0
    while (i < 100) do
        init = string.find(str, word, init, true)
        if init then
            init = init + 1
            amount = amount + 1
        else
            break
        end
        i = i + 1
    end
    return amount
end

function M.emptyFormat(s)
    local amount = StringUtil.findWord(s, "%s")
    if amount == 0 then
        return string.format(s)
    else
        return s
    end
    --local params = {}
    --for i = 1, amount do
    --    params[i] = string.empty
    --end
    --return string.format(s, table.unpack(params))
end

-- 拆字text，得到一组字符
function M.getWords(str)
    local skip = 0
    local chars = {}
    local ret = {}
    for i = 1, string.len(str) do
        local c = string.byte(str, i)
        if i > skip then
            if #chars > 0 then
                table.insert(ret, string.char(table.unpack(chars)))
            end
            chars = {}
            if c >= 128 then
                if c < 0xE0 then -- 2位utf8
                    skip = i + 1

                elseif 0xE0 <= c and c < 0xF0 then -- 3位utf8
                    skip = i + 2

                elseif 0xF0 <= c and c < 0xF8 then -- 4位utf8
                    skip = i + 3

                end
            end
        end
        table.insert(chars, c)
    end
    if #chars > 0 then
        table.insert(ret, string.char(table.unpack(chars)))
    end
    return ret
end


function M.getYMD(time)
    local t = os.date("*t", time)
    return Language.getString(100120, t.year, t.month, t.day)
end

function M.getH(time)
    local t = os.date("*t", time)
    return Language.getString(100121, t.hour)
end

function M.getSABC(index)
    if index == 1 then
       return Language.getString(268521)
    elseif index == 2 then
        return Language.getString(268522)
    elseif index == 3 then
        return Language.getString(268523)
    elseif index == 4 then
        return Language.getString(268524)
    end
    return ""
end

function M.getDailyActivityTime(activationConfig)
    local level = Cache.roleCache:getLevel()
    local worldLevel = Cache.roleCache:getWorldLev()
    local limitData = {}
    local str = activationConfig.activeDt
    local spiltArr = {}
    if string.isEmpty(activationConfig.activeDt) then
        printWarning(activationConfig.type.."的activeDt为空")
        return "活动时间为空"
    end
    local length = string.len(activationConfig.activeDt)
    local startIndex = 1
    local endIndex = 1
    if length == 0 then
        return
    end
    while (endIndex<length) do
        startIndex = string.find(str,"['[']",startIndex)
        endIndex = string.find(str,"]",endIndex)
        local str2 = string.sub(str,startIndex+1,endIndex-1)
        startIndex = startIndex+1
        endIndex = endIndex+1
        table.insert(spiltArr,str2)
    end
    if activationConfig.levelNoopen ~= "" then
        local sIndex = string.find(activationConfig.levelNoopen,"['[']")
        local eIndex = string.find(activationConfig.levelNoopen,"]")
        local str2 = string.sub(activationConfig.levelNoopen,sIndex+1,eIndex-1)
        local levelTab = string.split(str2,",")
        limitData.levelDay = tonumber(levelTab[1])
        limitData.levelLevel = tonumber(levelTab[2])
    end
    if activationConfig.worldLvNoopen ~= "" then
        local sIndex = string.find(activationConfig.worldLvNoopen,"['[']")
        local eIndex = string.find(activationConfig.worldLvNoopen,"]")
        local str2 = string.sub(activationConfig.worldLvNoopen,sIndex+1,eIndex-1)
        local worldTab = string.split(str2,",")
        limitData.worldDay = tonumber(worldTab[1])
        limitData.worldLevel = tonumber(worldTab[2])
    end
    --dump(52,limitData,"==================")
    --根据逗号进行拆分
    local openTime = ""
    local time = ""
    --dump(52,spiltArr,"字符串啊啊啊啊啊啊啊啊啊 啊啊")
    local weekTime = {}
    for _, v in pairs(spiltArr) do
        local isAdd = true
        local tab = string.split(v,",")
        local day = tonumber(tab[1])
        if limitData.levelDay and limitData.levelDay == day and limitData.levelLevel <= level then
            isAdd = false
        end
        if limitData.worldDay and limitData.worldDay == day and limitData.worldLevel <= worldLevel then
            isAdd = false
        end
        if isAdd then
            time = tab[2].."-"..tab[3]
            if not weekTime[time] then
                weekTime[time] = {}
            end
            table.insert(weekTime[time],day)
        else
            --dump(52,v,"不开启的天数：：：")
        end
    end
    --openTime = openTime..time
    --dump(52,weekTime,"每周的天数")
    local strs = {}
    for time, days in pairs(weekTime) do
        if #days == 7 then
            table.insert(strs,Language.getString(104008) .. " " .. time)
        else
            local str = ""
            for index, day in pairs(days) do
                str = str .. StringUtil.transWeekDayToChineseTwo(tonumber(day))
                if index <#days then
                    str = str .. "、"
                else
                    str = str .." "
                end
            end
            table.insert(strs,str..time)
        end
    end

    for key, value in pairs(strs) do
        openTime = openTime .. value
        if key < #strs then
            openTime = openTime .. "\n"
        end
    end
    return openTime
end

--剔除名字中的·前面的字符
function M.rejectPoint(str)
    local index = string.find(str,"·")
    local length = #str
    local arr = string.split(str,"·")
    if index > 0 then
        return string.sub(str,index+2,length)
    end
    return str
end

function M.deleteUbbColor(str)
    str = string.gsub(str, "%[color=.-%]", "")
    str = string.gsub(str, "%[/color%]", "")
    return str
end

rawset(_G, "StringTool", M)
StringTool = M