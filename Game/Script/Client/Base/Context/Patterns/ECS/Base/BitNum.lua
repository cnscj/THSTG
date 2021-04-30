---@class BitNum
local MAX_BIT_NUM = 64
local M = class("BitNum")

---@param n number@位数
function M:ctor(n)
    self.num1 = 0
    self.num2 = 0

    self._str = false
    self._isDirty = true
    self.isReadOnly = false

    if not n then
        return
    end
    if n < 0 then
        printError("[BitNum] n不应该为负数")
        return
    end
    
    if n < MAX_BIT_NUM then
        self.num1 = 1 << n
        return
    end
    if n < 2*MAX_BIT_NUM then
        self.num2 = 1 << (n - MAX_BIT_NUM)
        return
    end
    printErrorNoTraceback("[BitNum] 分配给component的位数不够了，加一下num3!!!")
end

---@param b BitNum@ self|b
function M:add(b)
    if self.isReadOnly then
        printError("[BitNum] 禁止修改")
        return
    end
    self.num1 = self.num1 | b.num1
    self.num2 = self.num2 | b.num2
    self._isDirty = true
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
    self._isDirty = true
    return self
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
    if self._isDirty then
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
    self._isDirty = true
end
--
local _readOnlyZero = false
local function _getReadOnlyZero()
    if not _readOnlyZero then 
        _readOnlyZero = M.new(0)
        _readOnlyZero.isReadOnly = true
    end
    return _readOnlyZero
end

M.Zero = _getReadOnlyZero()

return M