-- 标签类

---@class GTag:GComponent
local M = class("GTag", GComponent)

function M:ctor(obj)
end

function M:init(obj)
end


function M:setIcon2(url)
    self._obj:GetChild("icon2").url = url
end

function M:setText2(text)
    self._obj:GetChild("title2").text = text
end

function M:setIcon(url)
    self._obj.icon = url
end

function M:setText(text)
    text = OverseaConfig.isLabelShield(text, self._obj)
    text = text or ""
    self._obj.text = text
end

function M:getText()
    return self._obj.text
end

function M:setFontSize(size)
    self._obj.titleFontSize = size
end

function M:setColor(color)
    self._obj.color = color
end

function M:getTextField()
    return self._obj:GetTextField()
end


function M:setIconVisible(vis)
    self._obj.iconObj.visible = vis
end

function M:setTitleVisible(vis)
    self._obj.titleObj.visible = vis
end

function M:setStroke(val)
    self._obj.titleObj.stroke = val
end

function M:setStrokeColor(color)
    self._obj.titleObj.strokeColor = color
end

function M:getTitleObj()
    return self._obj.titleObj
end

function M:setEffectCode(code)
    local effect = self:getChild("effect", "Effect")
    if effect then
        effect:setEffectCode(code)
    end
end

-- 富文本的对齐方式
function M:setLineVAlign(align)
    local textField = self:getTextField()
    if textField then
        textField.lineVAlign = align
    end
end

function M:setVar(key, value)
    self._obj.titleObj:SetVar(key, value):FlushVars()
end

rawset(_G, "GTag", M)