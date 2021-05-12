---@class BitNum
local M = class("BitNum",nil,{
    MAX_BIT_NUM = 64,
    Zero = false,
    One = false
})

---@param n number@位数
function M:ctor(n)
    self.num1 = 0
    self.num2 = 0
    self.isReadOnly = false

    self._str = false

    self:bit(n)
end

function M:set(num1,num2)
    self.num1 = num1
    self.num2 = num2
end

function M:bit(n)
    if not n then
        return
    end

    if n < 0 then
        printError("[BitNum] n不应该为负数")
        return
    end

    if n < M.MAX_BIT_NUM then
        self.num1 = 1 << n
        return
    end

    if n < 2*M.MAX_BIT_NUM then
        self.num2 = 1 << (n - M.MAX_BIT_NUM)
        return
    end

    printErrorNoTraceback("[BitNum] 分配给value的位数不够了，加一下num3!!!")
end

---@param b BitNum@ self|b
function M:add(b)
    if self.isReadOnly then
        printError("[BitNum] 禁止修改")
        return
    end
    self.num1 = self.num1 | b.num1
    self.num2 = self.num2 | b.num2
    
    self._str = false
    return self
end

---@param b BitNum@ self & (~b)
function M:del(b)
    if self.isReadOnly then
        printError("[BitNum] 禁止修改")
        return
    end
    self.num1 = self.num1 & (~b.num1)
    self.num2 = self.num2 & (~b.num2)

    self._str = false
    return self
end

function M:equal(b)
    return self.num1 == b.num1 
        and self.num2 == b.num2
end

---@param b BitNum
---@return boolean@ self & b == b
function M:containAll(b)
    return self.num1 & b.num1 == b.num1
            and self.num2 & b.num2 == b.num2
end

---@param b BitNum
---@return boolean@ self & b ~= 0
function M:containAny(b)
    return self.num1 & b.num1 ~= 0
            or self.num2 & b.num2 ~= 0
end


function M:toString()
    if not self._str then
        self._str = string.format("%d|%d", self.num2, self.num1)
    end
    return self._str
end

function M:isZero()
    return self.num1 == 0 and self.num2 == 0
end

function M:clone()
    local i = M.new()
    i.num1 = self.num1
    i.num2 = self.num2
    return i
end

function M:clear()
    self.num1 = 0
    self.num2 = 0

    self._str = false
end

M.Zero = M.new(0)
M.Zero.isReadOnly = true

M.One = M.new(0)
M.One:set(2^M.MAX_BIT_NUM-1,2^M.MAX_BIT_NUM-1)
M.One.isReadOnly = true

rawset(_G, "BitNum", M)