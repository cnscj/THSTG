--计算代码参考Tolua的Quaternion.lua

local math	= math
local sin 	= math.sin
local cos 	= math.cos
local acos 	= math.acos
local asin 	= math.asin
local sqrt 	= math.sqrt
local min	= math.min
local max 	= math.max
local sign	= math.sign
--local atan2 = math.atan2  --集成的xlua有问题，没有这函数.
local abs	= math.abs

Quaternion = {__cname = 'Quaternion'}
setmetatable(Quaternion, Quaternion)


Quaternion.__index = function(t, k)
	return rawget(Quaternion, k)
end

Quaternion.__call = function(t, x, y, z, w)
	local v = {x = x or 0, y = y or 0, z = z or 0, w = w or 0}
	setmetatable(v, Quaternion)
	return v
end

Quaternion.__tostring = function(self)
	return string.format("(%0.1f, %0.1f, %0.1f, %0.1f)", self.x, self.y, self.z, self.w)
end

Quaternion.__div = function(va, d)
	return Quaternion(va.x / d, va.y / d, va.z / d, va.w / d)
end

Quaternion.__mul = function(lhs, rhs)
	if getmetatable(rhs) == Quaternion then
		return Quaternion((((lhs.w * rhs.x) + (lhs.x * rhs.w)) + (lhs.y * rhs.z)) - (lhs.z * rhs.y), (((lhs.w * rhs.y) + (lhs.y * rhs.w)) + (lhs.z * rhs.x)) - (lhs.x * rhs.z), (((lhs.w * rhs.z) + (lhs.z * rhs.w)) + (lhs.x * rhs.y)) - (lhs.y * rhs.x), (((lhs.w * rhs.w) - (lhs.x * rhs.x)) - (lhs.y * rhs.y)) - (lhs.z * rhs.z))
	elseif getmetatable(rhs) == Vector3 then
		return lhs:mulVec3(rhs)
	else
		error("Not support Quaternion to multiply with a number or some type else")
		return lhs
	end
end

Quaternion.__add = function(va, vb)
	return Quaternion(va.x + vb.x, va.y + vb.y, va.z + vb.z, va.w + vb.w)
end

Quaternion.__sub = function(va, vb)
	return Quaternion(va.x - vb.x, va.y - vb.y, va.z - vb.z, va.w - vb.w)
end

Quaternion.__unm = function(va)
	return Quaternion(-va.x, -va.y, -va.z, -va.w)
end

Quaternion.__eq = function(a, b)
	return a.x == b.x and a.y == b.y and a.z == b.z and a.w == b.w
end

function Quaternion:equals(x, y, z, w)
	return self.x == x and self.y == y and self.z == z and self.w == w
end

function Quaternion:set(x, y, z, w)
	self.x = x or 0
	self.y = y or 0
	self.z = z or 0
	self.w = w or 0
	return self
end

function Quaternion:clone()
	return Quaternion(self.x, self.y, self.z, self.w)
end

function Quaternion:pow2()
	return self.x * self.x + self.y * self.y + self.z * self.z + self.w * self.w
end

function Quaternion:mulVec3(point)
	local vec = Vector3()
    
	local num 	= self.x * 2
	local num2 	= self.y * 2
	local num3 	= self.z * 2
	local num4 	= self.x * num
	local num5 	= self.y * num2
	local num6 	= self.z * num3
	local num7 	= self.x * num2
	local num8 	= self.x * num3
	local num9 	= self.y * num3
	local num10 = self.w * num
	local num11 = self.w * num2
	local num12 = self.w * num3
	
	vec.x = (((1 - (num5 + num6)) * point.x) + ((num7 - num12) * point.y)) + ((num8 + num11) * point.z)
	vec.y = (((num7 + num12) * point.x) + ((1 - (num4 + num6)) * point.y)) + ((num9 - num10) * point.z)
	vec.z = (((num8 - num11) * point.x) + ((num9 + num10) * point.y)) + ((1 - (num4 + num5)) * point.z)
	
	return vec
end

function Quaternion:normalize()
	local clone = self:clone()
	clone:setNormalize()
	return clone
end

function Quaternion:setNormalize()
	local n = self:pow2()
	if n ~= 1 and n > 0 then
		n = 1 / sqrt(n)
		self.x = self.x * n
		self.y = self.y * n
		self.z = self.z * n
		self.w = self.w * n
	end
	return self
end

------------------Static Functions----------------
function Quaternion.dot(a, b)
	return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w
end

function Quaternion.lerp(q1, q2, t)
	t = math.clamp01(t)

	local q = Quaternion(0, 0, 0, 1)
	if Quaternion.dot(q1, q2) < 0 then
		q.x = q1.x + (-q2.x -q1.x) * t
		q.y = q1.y + (-q2.y -q1.y) * t
		q.z = q1.z + (-q2.z -q1.z) * t
		q.w = q1.w + (-q2.w -q1.w) * t
	else
		q.x = q1.x + (q2.x - q1.x) * t
		q.y = q1.y + (q2.y - q1.y) * t
		q.z = q1.z + (q2.z - q1.z) * t
		q.w = q1.w + (q2.w - q1.w) * t
	end
	return q:setNormalize()
end

function Quaternion.fromUnity(other)
	return Quaternion(other.x, other.y, other.z, other.w)
end

local _next = { 2, 3, 1 }

--Creates a rotation with the specified forward and upwards directions(Vector3).
function Quaternion.lookRotation(forward, up)
	local length = forward:length()
	if length < 1e-6 then
		error("error input forward to Quaternion.LookRotation"..tostring(forward))
		return nil
	end

	forward = forward / length
	up = up or Vector3.up
	local right = Vector3.cross(up, forward):normalize()
    up = Vector3.cross(forward, right)
    right = Vector3.cross(up, forward)

	local t = right.x + up.y + forward.z

	if t > 0 then
		local x, y, z, w
		t = t + 1
		local s = 0.5 / sqrt(t)
		w = s * t
		x = (up.z - forward.y) * s
		y = (forward.x - right.z) * s
		z = (right.y - up.x) * s
		
		local ret = Quaternion(x, y, z, w)
		return ret:setNormalize()
	else
		local rot = 
		{
			{right.x, up.x, forward.x},
			{right.y, up.y, forward.y},
			{right.z, up.z, forward.z},
		}
	
		local q = {0, 0, 0}
		local i = 1
		
		if up.y > right.x then
			i = 2
		end
		
		if forward.z > rot[i][i] then
			i = 3
		end
		
		local j = _next[i]
		local k = _next[j]
		
		local t = rot[i][i] - rot[j][j] - rot[k][k] + 1
		local s = 0.5 / sqrt(t)
		q[i] = s * t
		local w = (rot[k][j] - rot[j][k]) * s
		q[j] = (rot[j][i] + rot[i][j]) * s
		q[k] = (rot[k][i] + rot[i][k]) * s
		
		local ret = Quaternion(q[1], q[2], q[3], w)
		return ret:setNormalize()
	end
end

--get euler angle from a quaternion
local Mathf = CS.UnityEngine.Mathf
local atan2 = Mathf.Atan2
local rad2Deg = Mathf.Rad2Deg
local halfDegToRad = 0.5 * Mathf.Deg2Rad
local pi = Mathf.PI
local half_pi = pi * 0.5
local two_pi = 2 * pi
local negativeFlip = -0.0001
local positiveFlip = two_pi - 0.0001

--from http://www.geometrictools.com/Documentation/EulerAngles.pdf
--Order of rotations: YXZ
local function SanitizeEuler(euler)
	if euler.x < negativeFlip then
		euler.x = euler.x + two_pi
	elseif euler.x > positiveFlip then
		euler.x = euler.x - two_pi
	end

	if euler.y < negativeFlip then
		euler.y = euler.y + two_pi
	elseif euler.y > positiveFlip then
		euler.y = euler.y - two_pi
	end

	if euler.z < negativeFlip then
		euler.z = euler.z + two_pi
	elseif euler.z > positiveFlip then
		euler.z = euler.z + two_pi
	end
end
function Quaternion.toEulerAngles(quat)
	local x = quat.x
	local y = quat.y
	local z = quat.z
	local w = quat.w
		
	local check = 2 * (y * z - w * x)

	if check < 0.999 then
		if check > -0.999 then
			local v = Vector3( -asin(check), 
								atan2(2 * (x * z + w * y), 1 - 2 * (x * x + y * y)), 
								atan2(2 * (x * y + w * z), 1 - 2 * (x * x + z * z)) )
			SanitizeEuler(v)
			v:mul(rad2Deg)
			return v
		else
			local v = Vector3(half_pi, atan2(2 * (x * y - w * z), 1 - 2 * (y * y + z * z)), 0)
			SanitizeEuler(v)
			v:mul(rad2Deg)
			return v
		end
	else
		local v = Vector3(-half_pi, atan2(-2 * (x * y - w * z), 1 - 2 * (y * y + z * z)), 0)
		SanitizeEuler(v)
		v:mul(rad2Deg)
		return v
	end
end

--create a quaternion from euler angles using 'degree'
function Quaternion.euler(x, y, z)
	if y == nil and z == nil then
		y = x.y
		z = x.z
		x = x.x
	end
	
	x = x * 0.0087266462599716
    y = y * 0.0087266462599716
    z = z * 0.0087266462599716

	local sinX = sin(x)
    x = cos(x)
    local sinY = sin(y)
    y = cos(y)
    local sinZ = sin(z)
    z = cos(z)

    local q = { x = y * sinX * z + sinY * x * sinZ, 
    			y = sinY * x * z - y * sinX * sinZ, 
    			z = y * x * sinZ - sinY * sinX * z, 
    			w = y * x * z + sinY * sinX * sinZ }
	setmetatable(q, Quaternion)
	return q
end


-----using Unity implementation-------
--该函数性能超过纯lua实现，不太科学...
function Quaternion.lookRotation_Unity(forward, up)
	local quat
	if up then
		quat = CS.UnityEngine.Quaternion.LookRotation(forward, up)
	else
		quat = CS.UnityEngine.Quaternion.LookRotation(forward)
	end
	return Quaternion(quat.x, quat.y, quat.z, quat.w)
end

--该函数性能不如纯lua实现
function Quaternion.toEulerAngles_Unity( quat )
	--unity现在取消了静态函数，只能创建一个Quaternion再获取，效率降低.
	local quaternion = CS.UnityEngine.Quaternion(quat.x, quat.y, quat.z, quat.w)
	local rot = quaternion.eulerAngles
	return Vector3(rot.x, rot.y, rot.z)
end

--该函数性能不如纯lua实现
function Quaternion.euler_Unity( x,y,z )
	local quat = CS.UnityEngine.Quaternion.Euler(x, y, z)
	return Quaternion(quat.x, quat.y, quat.z, quat.w)
end

Quaternion.identity = Quaternion(0, 0, 0, 1)
Quaternion.zero = Quaternion.identity
Quaternion.one = Quaternion(1, 1, 1, 1)
