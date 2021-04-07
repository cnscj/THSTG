---@class GTextInput:GLabel
local M = class("GTextInput", GLabel)

function M:ctor(obj)
end

function M:init(obj)
end

function M:setText(text)
    self._obj.text = text
end

function M:getText()
    return self._obj.text
end


-- 设置默认显示名字
function M:setPlaceHolder(text)
    self._obj.promptText = text
end

-- 设置焦点
function M:requestFocus()
    self._obj:RequestFocus()
end



function M:onFocusIn(func)
    self._obj.onFocusIn:Add(func)
end

function M:onFocusOut(func)
    self._obj.onFocusOut:Add(func)
end

function M:onChanged(func)
    self._obj.onChanged:Add(func)
end



function M:setFocusIn(func)
    self._obj.onFocusIn:Set(func)
end

function M:setFocusOut(func)
    self._obj.onFocusOut:Set(func)
end

function M:setChanged(func)
    self._obj.onChanged:Set(func)
end


function M:onChangedCall()
    self._obj.onChanged:Call()
end

--设置最大输入字数
function M:setMaxLength( num )
   self._obj.maxLength = num
end

rawset(_G, "GTextInput", M)

-- editable
-- hideInput
-- maxLength
-- promptText
