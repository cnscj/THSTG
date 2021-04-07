---@class GLabel:GComponent
local M = class("GLabel", GComponent)

function M:ctor(obj)
    self.__text = false

    OverseaConfig.resetLabelShield(obj)
end

function M:init(obj)
end

function M:setText(text)
    text = OverseaConfig.isLabelShield(text, self._obj)
    text = text or ""
    if self.__text == text then
        return
    end

    if __DEBUG__ then
        if self.__text == false and self._isGetChild == 1 
            and (self._obj.text ~= nil and self._obj.text ~= "" and self._obj.text ~= "123" ) then
            printWarning(self._obj.text, "==->", text)
            local str = getTraceback()
            -- if not Cache.testCache:updateTracebackLabel(str) then
                printWarning("Label需勾选：发布时清除本文！！！相对应功能同学看看！！！")
                log(str)
            -- end
        end
    end

    self.__text = text
    self._obj.text = text
end

function M:getText()
    return self._obj.text
end

function M:setColor(color)
    self._obj.color = color
end

function M:getColor()
    return self._obj.color
end

function M:setStrokeColor(color)
	self._obj.strokeColor = color
end

function M:setstroke(stroke)
	self._obj.stroke = stroke
end

function M:setFontSize(fontSize)
    local textFormat = self._obj.textFormat
    textFormat.size = fontSize
    self._obj.textFormat = textFormat
end

function M:setFont(font)
    local textFormat = self._obj.textFormat
    textFormat.font = font
    self._obj.textFormat = textFormat
end

function M:setLetterSpacing(letterSpacing)
    local textFormat = self._obj.textFormat
    textFormat.letterSpacing = letterSpacing
    self._obj.textFormat = textFormat
end

function M:setLineSpacing(lineSpacing)
    local textFormat = self._obj.textFormat
    textFormat.lineSpacing = lineSpacing
    self._obj.textFormat = textFormat
end

function M:setUnderline(underline)
    local textFormat = self._obj.textFormat
    textFormat.underline = underline
    self._obj.textFormat = textFormat
end

function M:getFontSize()
    return self._obj.textFormat.size
end

function M:setMaxWidth(width)
    self._obj.maxWidth = width
end

function M:setTemplateVars(values)
    self._obj.templateVars = values
end

function M:setVar(key, value)
    self._obj:SetVar(key, value):FlushVars()
end


function M:setAutoSize(autoSizeType)
    self._obj.autoSize = autoSizeType
end

function M:setAlign( align )
    self._obj.align = align
end


-- 自定义外部方法

---设置根据字数自动调节字距的文本
function M:setGapText(text)
    local fontNum = string.utf8len(text)
    local fontSize = self:getFontSize()
    local gap = self:getTextGap(fontSize, fontNum)
    self:setLetterSpacing(gap)
    self:setText(text)
end

---根据字数和字体大小计算间距
function M:getTextGap(fontSize, fontNum)
    local maxWidth = fontSize * 4
    if fontNum <= 1 then
        return 0
    end
    local width = fontSize * fontNum
    if width < maxWidth then
        return math.floor((maxWidth - width) / (fontNum - 1))
    else
        return 0
    end
end


function M:showEffect(code, args)
    code = code or 3022014

    local isCenter = args and args.isCenter
    if self._obj.pivot.x == 0.5 and self._obj.pivot.y == 0.5 and self._obj.pivotAsAnchor == true then
        isCenter = true
    end

    local offsetX = (args and args.offsetX or 0) + self:getWidth() / 2
    local offsetY = (args and args.offsetY) or 0
    local scale = args and args.scale or 1
    if code == 3021015 then
        offsetX = 133
        scale = 0.99
    end

    if not isCenter then
        -- 默认左上角
        UI.playEffect({
            xy = Vector2(self:getX() + offsetX, self:getY() + self:getHeight() / 2 + offsetY),
            parent = self:getParent(),
            code = code,
            scale = scale,
        })
    else
        -- 锚点在中心
        UI.playEffect({
            parent = self:getParent(),
            code = code,
            xy = Vector2(self:getX(), self:getY()),
            scale = scale,
        })
    end

    -- 属性变化
    SoundUtil.playUISound("ui_valuechange")
end

--[[
_text
autoSize : AutoSizeType = {
    None-无
    Both-高度和宽度
    Height-高度
    Shrink-自由缩放
}

textFormat = {
    font : 字体（默认）
    size : 字体大小（默认12）
    color : 颜色（默认黑色）
    lineSpacing : 行距（默认3）
    letterSpacing : 字距（默认0）
    underline : 下划线
    italic : 斜体
    bold : 粗体
}

singleLine 是否单行
UBBEnabled 是否支持ubb

align 左右对齐 AlignType = {
    Left, 
    Center, 
    Right
}
verticalAlign 上下对齐 VertAlignType = {
    Top, 
    Middle, 
    Bottom
}

shadowOffset Vector2 投影
strokeColor Color 描边颜色
stroke int 描边大小
]]

local _defaultStyle = {
    size = 18,
    color = Color("#000000"),
    lineSpacing = 3,
    letterSpacing = 0,
    underline = false,
    italic = false,
    bold = false,
    font = false,
    singleLine = false,
    UBBEnabled = true,
    autoSize = FairyGUI.AutoSizeType.Both,
    align = FairyGUI.AlignType.Left,
    verticalAlign = FairyGUI.VertAlignType.Top,
}

function M:setTextAttr(params)
    local style = params.style or {}

    local finalStyle = TableUtil.mergeTable(style, _defaultStyle)
    self._obj.textFormat = {
        color = finalStyle.color,
        size = finalStyle.size,
        font = finalStyle.font,

        lineSpacing = finalStyle.lineSpacing,
        letterSpacing = finalStyle.letterSpacing,

        underline = finalStyle.underline,
        italic = finalStyle.italic,
        bold = finalStyle.bold,
    }

    self._obj.singleLine = finalStyle.singleLine
    self._obj.UBBEnabled = finalStyle.UBBEnabled
    self._obj.autoSize = finalStyle.autoSize
    self._obj.align = finalStyle.align
    self._obj.verticalAlign = finalStyle.verticalAlign

    if params.maxWidth then
        self._obj.maxWidth = params.maxWidth
    end
    if params.shadowOffset then
        self._obj.shadowOffset = params.shadowOffset
    end
    if params.strokeColor then
        self._obj.strokeColor = params.strokeColor
    end
    if params.stroke then
        self._obj.stroke = params.stroke
        -- 是一个坑
        -- if params.size then
        --     self._obj.size = params.size
        -- end
    end
    if params.text then
        self._obj.text = params.text
    end
end


rawset(_G, "GLabel", M)
