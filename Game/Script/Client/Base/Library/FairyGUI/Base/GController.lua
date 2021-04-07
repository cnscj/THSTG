---@class GController
local M = class("GController")

function M:ctor(obj)
    self._obj = obj or false
end

function M:init(obj)
end
function M:destroy()
    self._obj = false
end

function M:getObj()
    return self._obj
end

function M:setSelectedIndex(index)
    self._obj.selectedIndex = index
end

function M:setSelectedIndexLua(index)
    self:setSelectedIndex(index - 1)
end

function M:getSelectedIndex()
    return self._obj.selectedIndex
end

function M:getPreviousIndex()
    return self._obj.previsousIndex
end

function M:setSelectedName(name)
    --local id = self._obj:GetPageIdByName(name)
    --self._obj.selectedIndex = id
    self._obj.selectedPage = name
end

function M:setSelectedNameBoolean(bol)
    if bol then
        self:setSelectedName("yes")
    else
        self:setSelectedName("no")
    end
end

function M:getSelectedNameBoolean()
    local name = self:getSelectedName()
    return name == "yes"
end

function M:getSelectedName()
    local id = self:getSelectedIndex()
    return self._obj:GetPageName(id)
end

---获取页面数量
function M:getPageCount()
    return self._obj.pageCount
end

function M:addPage(name)
    self._obj:AddPage(name)
end

function M:onChanged(func)
    self._obj.onChanged:Add(func)
end

function M:clearPages()
    self._obj:ClearPages()
end

rawset(_G, "GController", M)
