---@class GButton:GComponent
local M = class("GButton", GComponent)

function M:ctor(obj)
    ---@type RedDot
    self.redDot = false
    ---@type Effect
    self.effect = false
    self.cdBar = false

    OverseaConfig.resetLabelShield(obj)
end

function M:init(obj, args)
    self.redDot = self:getChild("redDot", "RedDot")

    if args then
        if args.buttonMode then
            self:setMode(args.buttonMode)
        end
    end
end

function M:destroy()
    --销毁特效
    if self.effect then
        self.effect:destroy()
        self.effect = false
    end

    self:super("GComponent", "destroy")
end

function M:setRedDot(params, useCustomStyle)
    if self.redDot then
        self.redDot:setKeys(params, useCustomStyle)
    end
end

function M:getRedDotState()
    return self.redDot:getRedDotState()
end

function M:showRedDot(visible, customStyle)
    if self.redDot then
        self.redDot:showRedDot(visible, customStyle)
    end
end

function M:setRedDotXY(x,y)
    if self.redDot then
        if x and y then
            self.redDot:setXY(x, y)
        elseif x then
            self.redDot:setX(x)
        elseif y then
            self.redDot:setY(y)
        end
    end
end

function M:setEffectCode(effectCode, effectScale, effectParams)
    -- 编辑器做好effect
    local effect = self:getChild("effect", "Effect")
    effect:setEffectCode(effectCode, effectScale, effectParams)
end

function M:showEffect(effectCode, scaleX, scaleY)
    -- 根据effectCode来计算scaleX和scanleY
    -- print(5, "showEffect", effectCode)
    if effectCode then
        if effectCode == true then
            effectCode = 3022017
        end

        local cloneMaterial = false
        if effectCode == 3022017 then
            -- 150, 60
            scaleX = self._obj.width / 150
            scaleY = self._obj.height / 60
            cloneMaterial = true
        else
            -- print(9, "TODO: 根据effectCode和按钮大小内部设置scale effectCode=", effectCode)
        end

        -- 默认最顶层，基类放不了太多层级数据
        if not self.effect then
            self.effect = UI.newEffect({
                parent = self,
                center = true,
            })
        end
        if self.effect:getEffectCode() ~= effectCode then
            self.effect:setEffectCode(effectCode)
            if scaleX then
                self.effect:setScale(scaleX, scaleY)
            end
        end
        if cloneMaterial then
            self.effect:setCloneMaterial(cloneMaterial)
        end
    else
        if self.effect then
            self.effect:dispose()
            self.effect = false
        end
    end
end

function M:forceShowEffect(...)
    self:showEffect(...)
    if self.effect then
        self.effect:replay()
    end
end

function M:setCloneMaterial( val )
    if self.effect then
        self.effect:setCloneMaterial(val)
    end
end

function M:setDefaultIcon(url)
    self._obj.defaultIcon = url
end

function M:setIcon(url)
    self._obj.icon = url
end

-- 只是展示点击效果,不会触发click!!! 
function M:fireClick(downEffect, clickCall)
    if downEffect == nil then
        downEffect = false
    end
    if clickCall == nil then
        clickCall = false
    end
    self._obj:FireClick(downEffect, clickCall)
end

function M:setSelectedIcon(url)
    self._obj.selectedIcon = url
end

function M:setText(text)
    text = OverseaConfig.isLabelShield(text)
    self._obj.title = text
end

function M:getText()
    return self._obj.title
end

function M:setFontSize(size)
    self._obj.titleFontSize = size
end

function M:setColor(color)
    self._obj.color = color
end

function M:onChanged(func)
    self._obj.onChanged:Add(func)
end

--
function M:setSelected(selected, call)
    self._obj.selected = selected
    if call then
        self:call()
    end
end
function M:isSelected()
    return self._obj.selected
end

--[[
FairyGUI.ButtonMode
Common
Check
Radio
]]
function M:setMode(mode)
    self._obj.mode = mode
end
function M:getMode()
    return self._obj.mode
end

function M:setDownScale(scale)
    self._obj.downEffectValue = scale
end

-- 单选or多选 点击是否切换
function M:setChangeStateOnClick(bool)
    self._obj.changeStateOnClick = bool
end

function M:getIconObj()
    return self._obj.iconObj
end

function M:getTitleObj()
    return self._obj.titleObj
end

function M:stopStageEffect()
    self:onTouchBeginCapture(function(event)
        StageManager.stopEffect = true
    end)
end

-- 倒计时
function M:countdownMask(params)

    local maskType = params.maskType
    local cdValue = params.cdValue
    local cdMax = params.cdMax
    local endText = params.endText
    local cdText = params.cdText or Language.getString(110016)

    local component = "ProgressBar_mask"
    if maskType == false then
        component = "ProgressBar_nomask"
    end
    if not self.cdBar then
        self.cdBar = UI.newPackageComponent(
                {
                    parent = self,
                    package = "UI",
                    component = component,
                    compType = "ProgressBar",
                    size = self._obj.size,
                    xy = Vector2(0, 0),
                    pivot = Vector2(0, 0),
                    pivotAsAnchor = true,
                })
        self.cdBar:setTitleType(FairyGUI.ProgressTitleType.Formater)
        self.cdBar:setTitleFormater(function(val, max)
            self:setText(string.format(cdText, math.ceil(val)))
        end)
    end
    self:setEnabled(false)
    self.cdBar:setValueMax(cdValue, cdMax)
    self.cdBar:tweenValue(0, cdValue):OnComplete(function()
        self:setEnabled(true)
        self:setText(endText)
    end)
end

rawset(_G, "GButton", M)
