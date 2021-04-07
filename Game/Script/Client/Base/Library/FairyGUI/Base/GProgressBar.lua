---@class GProgressBar:GComponent
local M = class("GProgressBar", GComponent)

-- isSpecialBar:是否特殊样式进度条，如边缘为三角形、弧形等，设置为true时，请参考FGUI工程UI包中ProgressBar1的bar组件是如何制作的
function M:ctor(obj, isSpecialBar)
    self.bar = false
    self.title = false
    self.effect = false

    self._isSpecialBar = false
    self._effectCode = false---是否显示特效，为false时，true时使用默认的特效，number时显示对应code的特效
    self._effectScale = 1

    self._queue = {}
    self._isRuning = false

    self._effectOnce = false
    self._callBack = false

    self._duration = 0.5
end

function M:init(obj, isSpecialBar)
    if isSpecialBar then
        self.bar = self:getChild("bar"):getChild("bar", "Object")
        self.bar:setWidth(self:getWidth())
    else
        self.bar = self:getChild("bar")
        self.title = self:getChild("title", "Label")
    end
    self._isSpecialBar = isSpecialBar
end

function M:setDuration(dur)
    self._duration = dur
end

function M:setValue(value)
    self._obj.value = value
end

function M:setMax(max)
    self._obj.max = max
end

function M:getPercent()
    return self._obj.value / self._obj.max
end

function M:getValue()
    return self._obj.value
end

function M:getMax()
    return self._obj.max
end

function M:setValueMax(value, max)
    -- 先设置最大值，然后设置当前值，不然特效的位置无法准确定位
    self._obj:setValueMax(value, max)
end



----------
function M:tweenQueue(value, max, callBack)
    max = max or self:getMax()

    self._callBack = callBack or false

    local lastPer
    local lastMax

    local percent = value / max
    if #self._queue == 0 then
        lastPer = self._obj.value / self._obj.max
        lastMax = self._obj.max
        -- print(5, "percent lastPer", #self._queue, percent, lastPer)
    else
        local lastData = self._queue[#self._queue]
        lastPer = lastData.percent
        lastMax = lastData.max
        -- print(5, "percent lastPer", #self._queue, percent, lastPer)
    end

    print(5, "percent lastPer", percent, lastPer)
    if percent > lastPer then
        table.insert(self._queue, {
            value = value,
            max = max,
            percent = percent,
        })
    elseif percent < lastPer then
        table.insert(self._queue, {
            value = max,
            max = lastMax,
            percent = 1,
            zero = true,
        })
        table.insert(self._queue, {
            value = value,
            max = max,
            percent = percent,
        })
    elseif percent == lastPer then

        -- if lastMax ~= max then
            table.insert(self._queue, {
                value = max,
                max = lastMax,
                percent = 1,
                zero = true,
            })
        -- end

        table.insert(self._queue, {
            value = value,
            max = max,
            percent = percent,
        })
    end
    self:__startQueue()
end

function M:stopQueue()
    local tweener = FairyGUI.GTween.GetTween(self._obj, FairyGUI.TweenPropType.Progress)
    if tweener then
        tweener:Kill(false) -- fgui
    end
    self._queue = {}
    self._isRuning = false
end

function M:__startQueue()
    if self._isRuning == false then
        self:__runNext()
    end
end

function M:__runNext()
    if #self._queue == 0 then
        self._isRuning = false

        if self._callBack then
            self:_callBack()
            self._callBack = false
        end
    else
        self._isRuning = true
        local popData = table.remove(self._queue, 1)
        self:setMax(popData.max)

        local curPercent = self:getPercent()
        local popPercent = popData.percent
        local duration = popPercent - curPercent
        duration = duration * self._duration
        -- print(5, "duration", duration, popPercent, curPercent)

        -- self:setValue(0)
        self:setMax(popData.max)
        local tweener = self._obj:TweenValue(popData.value, duration)
        tweener:OnComplete(function()
            if popData.zero then
                self:setValue(0)
            end
            self:__runNext()
        end)
        -- end
    end
end
----------


----------
function M:tweenValue(value, duration, callback)
    local tweener = self._obj:TweenValue(value, duration)
    if callback then
        tweener:OnComplete(callback)
    end
    return tweener
end

function M:tweenMax(duration)
    local maxValue = self:getMax()
    return self:tweenValue(maxValue, duration)
end

function M:stopTween(complete)
    local tweener = FairyGUI.GTween.GetTween(self._obj, FairyGUI.TweenPropType.Progress)
    if tweener then
        if complete == nil then
            complete = false
        end
        tweener:Kill(complete) -- fgui
    end
end
----------







-- 进度特效显隐
-- @param   code    特效的code，为false时不显示
-- @param   scale   特效的缩放比
-- @param   cloneMaterial  是否克隆材质  当特效裁剪异常的时候使用这个
function M:showEffect(code, scale, cloneMaterial)
    if code == nil then code = false end
    if type(scale) ~= "number" then scale = 1 end

    self._effectScale = scale

    if self._effectCode ~= code then
        self._effectCode = code

        if self._effectCode == true then
            self._effectCode = 3022038
        end
        if self._effectCode then
            if self.effect then
                self.effect:dispose()
                self.effect = false
            end
            if not self.effect then
                self.effect = UI.newEffect({
                    parent = self,
                    center = true,
                })
            end
            self.effect:setEffectCode(self._effectCode)
            if cloneMaterial == true then
                self.effect:setCloneMaterial(true)
            end
            self.effect:setScale(scale)
        else
            if self.effect then
                self.effect:dispose()
                self.effect = false
            end
        end
    end
end


function M:setTitleVisible(value)
    if self.title then
        self.title:setVisible(value)
    end
end

-- 设置文本内容显示区类型，参考FairyGUI的ProgressTitleType
-- 增加了FairyGUI.ProgressTitleType.Formater类型，主要配合setTitleFormater接口使用
--[[
    Percent,
    ValueAndMax,
    Value,
    Max,
    Formater
]]
function M:setTitleType(value)
    self._obj.titleType = value
end

-- 设置文本内容区显示格式化函数，格式如下：
-- function(value, max)
--  return string.format("哈哈:%d/%d", value, max)
-- end

function M:setTitleFormater(func)
    if func then
        self._obj.titleType = FairyGUI.ProgressTitleType.Formater
        self._obj.titleFormater = func
    end
end

function M:playOnceEffect(code, scale)
    if not self._effectOnce then
        self._effectOnce = self:getChild("effect", "Effect")
    end
    if not self._effectOnce then
        printWarning("需要给该ProgressBar添加Effect控件")
    else
        self._effectOnce:setEffectCode(code, scale)
        self._effectOnce:replay()
    end
end

function M:setTitleColor(color)
    self.title:setColor(color)
end

rawset(_G, "GProgressBar", M)
