---@class Color@ 0-1f，不是0-255
Color = { __cname = 'Color' }
setmetatable(Color, Color)

local metatable = { __index = Color }
function Color.new(r, g, b, a)
    local v = { r = r or 0, g = g or 0, b = b or 0, a = a or 1 }
    setmetatable(v, metatable)
    return v
end

Color.__call = function(t, colStr)
    local len = #colStr
    local r = tonumber("0x" .. string.sub(colStr, 2, 3)) / 255
    local g = tonumber("0x" .. string.sub(colStr, 4, 5)) / 255
    local b = tonumber("0x" .. string.sub(colStr, 6, 7)) / 255
    local a = 1
    if len == 9 then
        a = tonumber("0x" .. string.sub(colStr, 8, 9))
    end
    return Color.new(r, g, b, a)
end

-- Color.__tostring = function(self)
-- 	return string.format("(%s, %s, %s, %s)", self.r, self.g, self.b, self.a)
-- end

-- Color.__add = function(a, b)
-- 	return Color.new(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a)
-- end

-- Color.__sub = function(a, b)
-- 	return Color.new(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a)
-- end

-- Color.__mul = function(a, d)
-- 	return Color.new(a.r * d, a.g * d, a.d * d, a.a * d)
-- end

-- Color.__div = function(a, d)
-- 	return Color.new(a.r / d, a.g / d, a.b / d, a.a / d)
-- end

-- Color.__eq = function(a, b)
-- 	return a.r == b.r and a.g == b.g and a.b == b.b and a.a == b.a
-- end

function Color:equals(r, g, b, a)
    return self.r == r and self.g == g and self.b == b and self.a == a
end

function Color:clear()
    self:set(1, 1, 1, 1)
end

function Color:set(r, g, b, a)
    self.r = r or 0
    self.g = g or 0
    self.b = b or 0
    self.a = a or 0
    return self
end

function Color:clone()
    return Color.new(self.r, self.g, self.b, self.a)
end

-- function Color.lerp(a, b, t)
-- 	t = math.clamp01(t)
-- 	return Color.new(a.r + t * (b.r - a.r), 
-- 					 a.g + t * (b.g - a.g), 
-- 					 a.b + t * (b.b - a.b), 
-- 					 a.a + t * (b.a - a.a) )
-- end

-- function Color.grayScale(c)
-- 	return 0.299 * c.r + 0.587 * c.g + 0.114 * c.b
-- end

Color.white = Color.new(1, 1, 1, 1)
Color.black = Color.new(0, 0, 0, 1)
Color.gray = Color.new(0.5, 0.5, 0.5, 1)
Color.red = Color.new(1, 0, 0, 1)
Color.green = Color.new(0, 1, 0, 1)
Color.blue = Color.new(0, 0, 1, 1)

--怪物被打颜色配置
Color.rimTb1 = {
    color1 = Color.white,
    color2 = Color.new(0.11320752, 0, 0, 1),
    param = Vector3(0.6, 5.83, 0.2),
}
Color.rimTb2 = {
    color1 = Color.new(1, 0.2545501, 0, 1),
    color2 = Color.black,
    param = Vector3(0.6, 5, 0.2),
}