--[[平滑器]]
local M = class("SmoothDamper")

function M:ctor()
    self._velocity = 0
end


--current: 当前的位置
--target: 我们试图接近的位置
--currentVelocity: 当前速度，这个值由你每次调用这个函数时被修改
--smoothTime: 到达目标的大约时间，较小的值将快速到达目标
--maxSpeed: 选择允许你限制的最大速度
--deltaTime: 自上次调用这个函数的时间。默认为 Time.deltaTime

function M:damp(current, target, smoothTime)
    local maxSpeed = 9999999
    local deltaTime = 0.016
    smoothTime = math.max(0.0001, smoothTime)
    local num1 = 2 / smoothTime
    local num2 = num1 * deltaTime
    local num3 = (1.0 / (1.0 + num2 + 0.479999989271164 * num2 * num2 + 0.234999999403954 * num2 * num2 * num2))
    local num4 = current - target
    local num5 = target
    local max = maxSpeed * smoothTime
    local num6 = math.clamp(num4, -max, max)
    target = current - num6
    local num7 = (self._velocity + num1 * num6) * deltaTime
    self._velocity = (self._velocity - num1 * num7) * num3
    local num8 = target + (num6 + num7) * num3
    if ((num5 - current > 0) == (num8 > num5)) then
        num8 = num5
        self._velocity = (num8 - num5) / deltaTime
    end
    return num8
end

function M:velocity()
    return self.velocity
end


rawset(_G, "SmoothDamper", M)