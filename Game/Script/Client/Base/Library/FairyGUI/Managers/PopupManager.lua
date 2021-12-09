local M = class("PopupManager")
function M:ctor( ... )

end

--[[
    sender,
    data,
    pos = Vector2(100, 100), 
    pivot = Vector2(0, 0), 
]]
function M:show(params)

end

function M:hide()

end

function M:isShow(toolTipId)

end

function M:getCurPopup()
    
end

function M:clear()

end
-------
function M:_showPopup(obj)
    FairyGUI.GRoot.inst:AddChild(obj)
end

function M:_hidePopup()
   
end


rawset(_G, "PopupManager", false)
PopupManager = M.new()