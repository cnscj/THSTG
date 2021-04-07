---@class GLoader:GObject
local M = class("GLoader", GObject)

function M:ctor(obj)
    if obj == nil then
        printError("obj is nil")
    end
    self._obj = obj
    self.__defaultUrl = false
    self.__hasBeenReset = false
    self.component = false
end

function M:init(obj)
end

-- 设置默认图标
function M:setShowErrorSign(bool)
    self._obj.showErrorSign = bool
end

function M:setDefaultUrl(url)
    if self.__defaultUrl ~= url then
        self.__defaultUrl = url
        self._obj.defaultUrl = url
    end
end

function M:setUIUrl(package, component)
    local url = UIPackageManager.getUIURL(package, component)
    self._obj.url = url
end

--isSync: 是否同步加载。默认是异步，值为nil，只有同步时才会设置为true。
function M:setUrl(url, isSync, assetType)
    if __USE_FGUI_AB__ == true then
        self:checkResize(url)
    end

    self._obj.isAsyncLoading = not isSync  --这一行要在设置url之前，因为设置url时马上就加载资源了。

    --新加的字段，老的版本可能没有该字段
    if assetType and self._obj.set_assetType then
        self._obj.assetType = assetType  --指定加载的资源类型
    end

    self._obj.url = url
end

function M:getUrl()
    return self._obj.url
end

function M:setTexture(texture)
    self._obj.texture = texture
end

function M:getTexture()
    return self._obj.texture
end

function M:setColor(color)
    self._obj.color = color
end

function M:setPrecent(value)
    self._obj.fillAmount = value
end

local _defaultStyle = {
    url = false,
    -- align = FairyGUI.AlignType.Left,
    -- verticalAlign = FairyGUI.VertAlignType.Top,
    fill = FairyGUI.FillType.None,
    shrinkOnly = false,
    autoSize = true,

    fillMethod = FairyGUI.FillMethod.None,
    fillOrigin = 0,
    fillClockwise = false,
    -- fillAmount = 0
}

--[[
public enum AlignType
{
    Left, 
    Center, 
    Right
}
public enum VertAlignType
{
    Top, 
    Middle, 
    Bottom
}
public enum FillType
{
    None, 
    Scale, 
    ScaleMatchHeight, 
    ScaleMatchWidth, 
    ScaleFree, 
    ScaleNoBorder
}
]]

function M:setLoaderAttr(params)
    -- dump(5, params, "params")
    params = params or {}
    local style = params or {}
    local finalStyle = TableUtil.mergeTable(style, _defaultStyle)

    -- dump(5, finalStyle, "finalStyle")
    if finalStyle.url then
        self:setUrl(finalStyle.url)
    end
    -- self._obj.url = finalStyle.url
    -- align = finalStyle.align
    -- verticalAlign = finalStyle.verticalAlign
    self._obj.fill = finalStyle.fill
    self._obj.shrinkOnly = finalStyle.shrinkOnly
    self._obj.autoSize = finalStyle.autoSize

    self._obj.fillMethod = finalStyle.fillMethod
    self._obj.fillOrigin = finalStyle.fillOrigin
    self._obj.fillClockwise = finalStyle.fillClockwise

    if finalStyle.align then
        self._obj.align = finalStyle.align
    end
    if finalStyle.verticalAlign then
        self._obj.verticalAlign = finalStyle.verticalAlign
    end
end

function M:setFill(fill)
    self._obj.fill = fill
end

function M:getFill()
    return self._obj.fill
end

function M:checkResize(url)
    --设置了自动大小的
    --没设置自动大小但是也没设置自适应的
    --已经设置过的
    if self._obj.autoSize == true
            or self._obj.fill == FairyGUI.FillType.None
            or self.__hasBeenReset == true
    then
        if url and url:find("ui/single/") then
            local src = string.split(url, "|")[2]
            if src then
                local size = MapConfig:getImageSize(src)
                if size then
                    self:setSingleImage(size)
                else
                    printWarning("在single 却没有找到size", url)
                end
            end
        end
    end
end

function M:setSingleImage(size)
    self._obj.autoSize = false
    self._obj.fill = 4

    local width = size // 10000
    local height = size % 10000
    self:setSize(width, height)
    self.__hasBeenReset = true
end

-- loader加载的是component而不是图片
function M:getComponent()
    if not self._obj then
        return false
    end
    if self.component and self.component:getObj() ~= self._obj.component then
        self.component:dispose()
        self.component = false
    end
    if not self.component then
        local gComponent = self._obj.component
        if not gComponent then
            return false
        end
        self.component = GComponent.new(gComponent)
    end
    return self.component
end

-- 设置水平对齐方式，AlignType(Left, Center, Right)
function M:setAlign(value)
    self._obj.align = value
end

-- 设置垂直对齐方式，VertAlignType(Top, Middle, Bottom)
function M:setVerticalAlign(value)
    self._obj.verticalAlign = value
end

--设置图片九宫格
-- function M:setScale9Grid(rect,right,top,bottom)
--     if rect and right and top and bottom then
--         rect = Rect(rect,right,top,bottom)
--     end
--     self._obj.image.scale9Grid = rect
-- end

-- function M:getScale9Grid()
--     return self._obj.image.scale9Grid
-- end

rawset(_G, "GLoader", M)
