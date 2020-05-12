--[[
	数学库中的这些函数废弃了： atan2， cosh， sinh， tanh， pow， frexp， 以及 ldexp 。 
	你可以用 x^y 替换 math.pow(x,y)； 
	你可以用 math.atan 替换 math.atan2，
	前者现在可以接收一或两个参数； 
	你可以用 x * 2.0^exp 替换 math.ldexp(x,exp)。 
]]

--这里兼容一下
function math.pow(n,a)
	return n^a
end

function math.mod(nNum, m)
	local _, current = math.modf(nNum / m)
	current = current * m
	if nNum ~= 0 and current == 0 then
		current = 10
	end
	return tonumber(tostring(current))
end

function math.newrandomseed()
    math.randomseed(tostring(os.time()):reverse():sub(1, 6))
end

--四舍入伍，保留小数点后n位数
function math.round(value, n)
	n = n or 0
	local num = 10 ^ n
	value = checknumber(value)
    return math.floor(value * num + 0.5) / num
end

local pi_div_180 = math.pi / 180
function math.angle2radian(angle)
    return angle * pi_div_180
end

local pi_mul_180 = math.pi * 180
function math.radian2angle(radian)
    return radian / pi_mul_180
end

math.rad2Deg = 180 / math.pi


function math.clamp(value, min, max)
    if value < min then value = min end
    if value > max then value = max end
    return value
end

function math.clamp01(value)
    return math.clamp(value, 0, 1)
end

local function clampAngle(angle)
    if angle > 180 then
        angle = angle - math.ceil(angle / 360) * 360
    elseif angle < -180 then
        angle = angle - math.floor(angle / 360) * 360
    end
    return angle
end
function math.clampAngle(angle)
    return clampAngle(clampAngle(angle))
end

function math.lerp(from, to, t)
    return from + (to - from) * math.clamp01(t)
end

function math.sign(num)
    if num > 0 then
        return 1
    elseif num < 0 then
        return -1
    else 
        return 0
    end
end

function math.atan2(y, x)
	if x == 0 then
		if y == 0 then
			return 0
		elseif y > 0 then
			return math.pi / 2
		elseif y < 0 then
			return -math.pi / 2
		end
	end

	if x > 0 then
		if y == 0 then
			return 0
		elseif y > 0 then
			return math.atan(y/x)
		elseif y < 0 then
			return math.atan(y/x)
		end
	end

	if x < 0 then
		if y == 0 then
			return math.pi
		elseif y > 0 then
			return math.atan(y/x) + math.pi
		elseif y < 0 then
			return math.atan(y/x) - math.pi
		end
	end
	
	return math.atan(y/x)
end

