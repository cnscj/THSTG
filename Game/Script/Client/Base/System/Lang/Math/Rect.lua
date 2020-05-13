
Rect = {__cname = 'Rect'}
setmetatable(Rect, Rect)

Rect.__index = function(t, k)
	return rawget(Rect, k)
end

Rect.__call = function(t, x, y, width, height)
	local v = {x = x or 0, y = y or 0, width = width or 0, height = height or 0}
	setmetatable(v, Rect)
	return v
end

Rect.__tostring = function(self)
	return string.format("(%0.1f, %0.1f, %0.1f, %0.1f)", self.x, self.y, self.width, self.height)
end

function Rect:addVector2(x, y)
	return Rect(self.x + x, self.y + y, self.width, self.height)
end

function Rect:inSpan(x, y)
	return self:contains(x, y)
end
function Rect:contains(x, y)
	if (x >= self.x) and (x <= self.x + self.width) and
		(y >= self.y) and (y <= self.y + self.height) then
		return true
	end
	return false
end

function Rect:intersect(rect)
	if (rect.x <= self.x + self.width)
		and (rect.x + rect.width >= self.x)
		and (rect.y <= self.y + self.height)
		and (rect.y + rect.height >= self.y)
	then
		return true
	end
	return false
end

Rect.zero = Rect(0, 0, 0, 0)
