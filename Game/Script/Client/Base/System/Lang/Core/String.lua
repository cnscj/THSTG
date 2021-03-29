local base = _G

string._htmlspecialchars_set = {}
string._htmlspecialchars_set_ex = {k = "&", v = "&amp;"}
string._htmlspecialchars_set["\""] = "&quot;"
-- string._htmlspecialchars_set["'"] = "&#039;"
string._htmlspecialchars_set["<"] = "&lt;"
string._htmlspecialchars_set[">"] = "&gt;"
-- string._htmlspecialchars_set["+"] = " "

function string.htmlspecialchars(input)
    input = string.gsub(input, string._htmlspecialchars_set_ex.k, string._htmlspecialchars_set_ex.v)
    for k, v in pairs(string._htmlspecialchars_set) do
        input = string.gsub(input, k, v)
    end
    return input
end

function string.restorehtmlspecialchars(input)
    for k, v in pairs(string._htmlspecialchars_set) do
        input = string.gsub(input, v, k)
    end
    input = string.gsub(input, string._htmlspecialchars_set_ex.v, string._htmlspecialchars_set_ex.k)
    return input
end

function string.nl2br(input)
    return string.gsub(input, "\n", "<br />")
end

function string.text2html(input)
    input = string.gsub(input, "\t", "    ")
    input = string.htmlspecialchars(input)
    input = string.gsub(input, " ", "&nbsp;")
    input = string.nl2br(input)
    return input
end

-- function string.split(input, delimiter)
--     input = tostring(input)
--     delimiter = tostring(delimiter)
--     if (delimiter=='') then return false end
--     local pos,arr = 0, {}
--     -- for each divider found
--     for st,sp in function() return string.find(input, delimiter, pos, true) end do
--         table.insert(arr, string.sub(input, pos, st - 1))
--         pos = sp + 1
--     end
--     table.insert(arr, string.sub(input, pos))
--     return arr
-- end

function string.ltrim(input)
    return string.gsub(input, "^[ \t\n\r]+", "")
end

function string.rtrim(input)
    return string.gsub(input, "[ \t\n\r]+$", "")
end

function string.trim(input)
    input = string.gsub(input, "^[ \t\n\r]+", "")
    return string.gsub(input, "[ \t\n\r]+$", "")
end

function string.ucfirst(input)
    return string.upper(string.sub(input, 1, 1)) .. string.sub(input, 2)
end

local function urlencodechar(char)
    return "%" .. string.format("%02X", string.byte(char))
end
function string.urlencode(input)
    -- convert line endings
    input = string.gsub(tostring(input), "\n", "\r\n")
    -- escape all characters but alphanumeric, '.' and '-'
    input = string.gsub(input, "([^%w%._%- ])", urlencodechar)
    -- convert spaces to "+" symbols
    return string.gsub(input, " ", "+")
end

function string.urldecode(input)
    input = string.gsub (input, "+", " ")
    input = string.gsub (input, "%%(%x%x)", function(h) return string.char(checknumber(h,16)) end)
    input = string.gsub (input, "\r\n", "\n")
    return input
end

function string.utf8len(input)
    return utf8.len(input)
end

--带中文的字符串截取方法
function string.utf8sub(input, beginIdx, endIdx)
    if type(beginIdx) ~= "number" then
        return input
    end

    local utf8len = utf8.len(input)

    if beginIdx < 0 then
        beginIdx = utf8len + beginIdx + 1
    end

    if type(endIdx) ~= "number" then
        endIdx = utf8len
    elseif endIdx < 0 then
        endIdx = utf8len + endIdx + 1
    end


    local idx = 0
    local from, to = 0, 0
    for p, c in utf8.codes(input) do
        idx = idx + 1
        if idx == beginIdx then
            from = p
        end
        if idx >= endIdx then
            to = p + string.len(utf8.char(c)) - 1
            break
        end
    end

    return string.sub(input, from, to)
end

function string.formatnumberthousands(num)
    local formatted = tostring(checknumber(num))
    local k
    while true do
        formatted, k = string.gsub(formatted, "^(-?%d+)(%d%d%d)", '%1,%2')
        if k == 0 then break end
    end
    return formatted
end

--将unicode码的字符串转换成utf8编码的字符串
function string.unicode2utf8(convertStr)
    if type(convertStr)~="string" then
        return convertStr
    end

    local resultStr=""
    local i=1
    while true do
        local num1=string.byte(convertStr,i)
        local unicode

        if num1~=nil and string.sub(convertStr,i,i+1)=="\\u" then
            unicode=tonumber("0x"..string.sub(convertStr,i+2,i+5))
            i=i+6
        elseif num1~=nil then
            unicode=num1
            i=i+1
        else
            break
        end

        if unicode <= 0x007f then
            resultStr=resultStr..string.char(unicode & 0x7f)
        elseif unicode >= 0x0080 and unicode <= 0x07ff then
            resultStr=resultStr..string.char(0xc0 | ((unicode >> 6) & 0x1f))
            resultStr=resultStr..string.char(0x80 | (unicode & 0x3f))
        elseif unicode >= 0x0800 and unicode <= 0xffff then
            resultStr=resultStr..string.char(0xe0 | ((unicode >> 12) & 0x0f))
            resultStr=resultStr..string.char(0x80 | ((unicode >> 6) & 0x3f))
            resultStr=resultStr..string.char(0x80 | (unicode & 0x3f))
        end
    end
    resultStr=resultStr..'\0'
    
    return resultStr
end

--将utf8编码的字符串转换成unicode编码的字符串
function string.utf82unicode(convertStr)
    if type(convertStr)~="string" then
        return convertStr
    end
    
    local resultStr=""
    local i=1
    local num1=string.byte(convertStr,i)
    while num1~=nil do
        if num1 >= 0x00 and num1 <= 0x7f then
            tempVar1=num1
            tempVar2=0
            resultStr=resultStr..string.sub(convertStr,i,i)
        else
            local tempVar1,tempVar2
            if (num1 & 0xe0) == 0xc0 then
                local t1 = 0
                local t2 = 0
                t1 = (num1 & (0xff >> 3))
                i=i+1
                num1=string.byte(convertStr,i)
                t2 = (num1 & (0xff >> 2))
                tempVar1 = (t2 | ((t1 & (0xff >> 6)) << 6))
                tempVar2 = (t1 >> 2)
            elseif (num1 & 0xf0)== 0xe0 then
                local t1 = 0
                local t2 = 0
                local t3 = 0
                t1 = (num1 & (0xff >> 3))
                i=i+1
                num1=string.byte(convertStr,i)
                t2 = (num1 & (0xff >> 2))
                i=i+1
                num1=string.byte(convertStr,i)
                t3 = (num1 & (0xff >> 2))
                tempVar1 = (((t2 & (0xff >> 6)) << 6) | t3)
                tempVar2 = ((t1 << 4) | (t2 >> 2))
            end
            resultStr=resultStr..string.format("\\u%02x%02x",tempVar2,tempVar1)
        end
        i=i+1
        num1=string.byte(convertStr,i)
    end
    
    return resultStr
end

function string.totable(str)
    local loadfn = base.loadstring;
    local succeed, fn = pcall(loadfn, "return " .. str);
    if (not succeed) then
        printWarning("totable error! " .. str);
        return;
    end

    local tb;
    succeed, tb = pcall(fn);
    if (not succeed) then
        printWarning("totable failed! " .. str);
        return;
    end

    return tb;
end

--分割字符窜为数组
function string.split(str, sep)
    local sep, fields = sep or ":", {}
    if not str then return fields end
    
    local pattern = string.format("([^%s]+)", sep)
    string.gsub(str, pattern, function(c) fields[#fields+1] = c end)
    return fields
end

-- sep1 = "#"
-- sep2 = ","
function string.split2Times(str, sep1, sep2)
    local endTable = {}
    local strList = string.split(str, sep1)
    if strList and next(strList) then
        for _, tempStr in ipairs(strList) do
            local tempStrList = string.split(tempStr, sep2)
            if tempStrList and next(tempStrList) then
                table.insert(endTable, tempStrList)
            end
        end
    end
    return endTable
end

function string.splitEx(str,delimiter)
    if str==nil or str=='' or delimiter==nil then
        --print("error Happen in :",str," ,delimiter : "..delimiter)
        return nil
    end

    local result = {}
    ---printTable((str..delimiter):gmatch("(.-)"..delimiter))
    for match in string.gmatch((str..delimiter),"(.-)"..delimiter) do
        table.insert(result, match)
    end
    return result
end

--分割字符串  自动转换元素为number
function string.split2Num( str, sep )
    if not str or not sep then
        return
    end
    local fields = {}
    local pattern = string.format("([^%s]+)", sep)
    string.gsub(str, pattern, function(c) fields[#fields+1] = tonumber(c) end)
    return fields
end

string.Empty = ""
function string.isEmpty(str)
    return not str or string.Empty == str
end