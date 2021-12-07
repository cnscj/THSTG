local DEBUG_UI = __DEBUG__ and __PRINT_TRACE_BACK__

---@class GObject
local M = class("GObject")

function M:ctor(obj, args)
    self._obj = obj or false

    --用来标记是否被destroy()过，理论上来说true的时候该类就不能再使用了，
    --主要是有些GObject被其它类引用到_children列表，或者其它成员变量，并且没有释放引用，而误destroy()。
    --但实际上，这些被引用的GObject应该独立destroy()，比如被放进了对象池，或者单例。此时，这个变量可以用来判断该类是否被destroy()过，而决定要不要再缓存它。
    self._isDestroyed = false

    --父节点
    self._parent = args and args.parent or false

    self._graph = false
    self._tweenerHelper = false
    self._xy = false

    self._uniqueOnClick = false
    self._clickFunc = false

    if DEBUG_UI then
        self._traceStr = string.empty
    end
end

function M:init(obj)

end

--清空该类的引用
--NOTE: 如果要进对象池的对象，必须在_exit()或者手动释放掉引用，不能让FView自动调用进池对象的destroy()
function M:destroy()

    self:clearClick()

    if self._graph then
        self._graph:Dispose()
        self._graph = false
    end
    if self._tweenerHelper then
        self._tweenerHelper:destroy()
        self._tweenerHelper = false
    end

    self:_destroySelf()

    self._isDestroyed = true
end
function M:_destroySelf()
    for k, v in pairs(self) do
        -- print(5, k, v)
        if k ~= "_parent" and v ~= self and type(v) == "table" and type(v.destroy) == "function" then
            if type(v.isTypeOf) == "function" and v:isTypeOf("FView") then
                --do nothing
                print(9, "FView can't destroy: ", k, v.__cname)
            else
                v:destroy()
            end
        end

        if k ~= "__cname" then
            self[k] = false
        end
    end
end

function M:getObj()
    return self._obj
end


function M:getId()
    return self._obj.id
end

function M:isDestroyed()
    return self._isDestroyed
end

function M:getParent(compType)
    if not self._parent then
        self._parent = UIManager:convertComponent(self._obj.parent,compType)
    end
    return self._parent
end

function M:getParentGO()
    return self._obj.parent
end

function M:debugUI()
    if __DEBUG__ then
        local size = self._obj.size
        if self._graph then
            self._graph:Dispose()
            self._graph = false
        end

        if self._graph == false then
            self._graph = FairyGUI.GGraph()
            self._graph:DrawRect(size.x, size.y, 5, Color("#ff0000ff"), Color("#00000000"))
            self._graph.touchable = false
            if self._obj.AddChild then
                self._obj:AddChild(self._graph)
            end
        end
    end
end

function M:setBaseAttr(params)
    params = params or {}

    if params.sortingOrder and params.sortingOrder > 0 then
        self._obj.sortingOrder = params.sortingOrder
    end

    if params.xy then
        self._obj.xy = params.xy
    end

    if params.size then
        self._obj.size = params.size
    end

    if params.pivot then
        self._obj.pivot = params.pivot
    end
    if params.pivotAsAnchor then
        self._obj.pivotAsAnchor = params.pivotAsAnchor
    end
    if params.scaleX then
        self._obj.scaleX = params.scaleX
    end
    if params.scaleY then
        self._obj.scaleY = params.scaleY
    end

    if params.parent then
        params.parent:addChild(self)
    end
    if params.center then
        self._obj:Center()
    end
    if params.alpha then
        self:setAlpha(params.alpha)
    end

    if params.offset then
        params.offset.x = params.offset.x or 0
        params.offset.y = params.offset.y or 0
        self._obj.xy = self._obj.xy + params.offset
    end

    if params.touchable ~= nil then
        self:setTouchable(params.touchable)
    end

    if params.draggable ~= nil then
        self:setDraggable(params.draggable)
    end
end

function M:removeRelation(target, relationType)
    self._obj:RemoveRelation(target:getObj(), relationType)
end


-- x
function M:setX(x)
    self._obj.x = x
end
function M:getX()
    return self._obj.x
end
-- y
function M:setY(y)
    self._obj.y = y
end
function M:getY()
    return self._obj.y
end

-- x, y
function M:setXY(x, y)
    if not self._xy then
        self._xy = Vector2.zero
    end
    self._xy.x = x
    self._xy.y = y
    self._obj.xy = self._xy
end
function M:getXY()
    if not self._xy then
        self._xy = Vector2.zero
    end
    self._xy.x = self._obj.x
    self._xy.y = self._obj.y
    return self._xy
end

--setXY 会制造一个Vector2，完全没有必要
function M:setPos(x, y)
    self._obj:SetXY(x, y)
end

function M:setZ(z)
    self._obj.z = z
end

function M:getPosition()
    return self._obj.position
end

function M:shiftX(x)
    self._obj.x = self._obj.x + x
end

function M:shiftY(y)
    self._obj.y = self._obj.y + y
end

-- 与锚点无关,xy总是在左上角
function M:getCenter()
    local size = self:getSize()
    return Vector2(self._obj.x + size.x / 2, self._obj.y + size.y / 2)
end

-- 大小
function M:setWidth(width)
    self._obj.width = width
end
function M:getWidth()
    return self._obj.width
end
function M:setHeight(height)
    self._obj.height = height
end
function M:getHeight()
    return self._obj.height
end

function M:setMaxWidth(width)
    self._obj.maxWidth = width
end
function M:setMaxHeight(height)
    self._obj.maxHeight = height
end
function M:setMinWidth(width)
    self._obj.minWidth = width
end
function M:setMinHeight(height)
    self._obj.minHeight = height
end
function M:setInitWidth()
    self._obj.width = self._obj.initWidth
end
function M:getInitWidth()
    return self._obj.initWidth
end
function M:setInitHeight()
    self._obj.height = self._obj.initHeight
end
function M:getInitHeight()
    return self._obj.initHeight
end

function M:setViewHeight(height)
    self._obj.viewHeight = height
end
function M:getViewHeight()
    return self._obj.viewHeight
end

function M:getSize()
    return Vector2(self._obj.width, self._obj.height)
end

function M:setSize(width, height)
    self._obj:SetSize(width, height)
end

function M:getSourceSize()
    return Vector2(self._obj.sourceWidth, self._obj.sourceHeight)
end

-- 轴心
function M:setPivot(x, y, asAnchor)
    self._obj:SetPivot(x, y, asAnchor)
end
function M:getPivot()
    return self._obj.pivot
end
function M:isPivotAsAnchor()
    return self._obj.pivotAsAnchor
end

function M:getCompCenter()
    local x, y
    local w, h = self:getWidth(), self:getHeight()
    local pivot = self:getPivot()
    if self:isPivotAsAnchor() then
        x = w * (0.5 - pivot.x)
        y = h * (0.5 - pivot.y)
    else
        x = w * 0.5
        y = h * 0.5
    end
    return x, y
end

-- 数据
function M:setData(data)
    self._obj.data = data
end
function M:getData(data)
    return self._obj.data
end

-- 设置拖拽
function M:setDraggable(able)
    self._obj.draggable = able
end
function M:getDraggable()
    return self._obj.draggable
end

function M:isOnStage()
    return self._obj.onStage
end

-- 显示
function M:setVisible(visible)
    if visible == self:isVisible() or not self._obj then
        return
    end
    self._obj.visible = visible
end
function M:isVisible()
    if not self._obj then
        return false
    end
    return self._obj.visible
end
function M:setVisibleForce(visible)
    self._obj.visible = visible
end

-- function M:hideAndshrink()
--  self._obj.visible = false
--  self._obj.maxHeight = 1
--  self._obj.maxWidth = 1
-- end
-- function M:showAndSpread()
--  self._obj.visible = true
--  self._obj.maxHeight = 0
--  self._obj.maxWidth = 0
-- end

function M:setSortingOrder(order)
    self._obj.sortingOrder = order
end
function M:getSortingOrder()
    return self._obj.sortingOrder
end

-- 是否灰显
function M:setGrayed(grayed)
    self._obj.grayed = grayed
end
function M:getGrayed()
    return self._obj.grayed
end

-- 是否可点击
function M:setTouchable(able)
    self._obj.touchable = able
end

function M:getTouchable()
    return self._obj.touchable
end

-- 是否可用，变灰、不可触摸
function M:setEnabled(enable)
    self._obj.grayed = not enable
    self._obj.touchable = enable
end

function M:isEnabled()
    return self._obj.touchable
end

---------- 点击 -----------
function M:onClick(func)
    if self._uniqueOnClick == func then
        return
    end
    if not func then
        self:clearClick()
        return
    end
    --放后面，可能func = nil
    self._uniqueOnClick = func

    if self._traceStr then
        -- self._traceStr = getTraceback("", 1)
    end

    local path = FGUIUtil.getObjPath(self)

    if not DEBUG_UI then
        self._clickFunc = func
        self._obj.onClick:Set(self._clickFunc)
        return
    end
    if not self._clickFunc then
        self._clickFunc = function(...)
            if DEBUG_UI then
                if Cache.guideCache:isMarkUI() then
                    Cache.guideCache:saveMarkUIId(self)
                end
                printStack(self._uniqueOnClick, self._traceStr)
            end

            if self._uniqueOnClick then
                self._uniqueOnClick(...)
            end
        end
    end
    self._obj.onClick:Set(self._clickFunc)
end

function M:getOnClick()
    return self._uniqueOnClick
end

function M:addClick(func)
    self._obj.onClick:Add(func)
end

function M:removeClick(func)
    if self._obj then
        if self._uniqueOnClick == func then
            self._uniqueOnClick = false
            self._obj.onClick:Remove(self._clickFunc)
        else
            self._obj.onClick:Remove(func)
        end
    end
end

function M:clearClick()
    if self._clickFunc then
        Cache.guideCache:removeUI(self)
    end
    self._clickFunc = false

    self._uniqueOnClick = false
    if self._obj then
        self._obj.onClick:Clear()
    end
end

function M:onClickLink(func)
    self._obj.onClickLink:Add(func)
end

function M:call()
    self._obj.onClick:Call()
end

---------- 触摸 -----------
function M:onTouchBeginCapture(func)
    self._obj.onTouchBegin:AddCapture(func)
end

function M:onTouchMoveCapture(func)
    self._obj.onTouchMove:AddCapture(func)
end

function M:onTouchEndCapture(func)
    self._obj.onTouchEnd:AddCapture(func)
end

---------- 触摸 -----------
function M:onTouchBegin(func)
    self._obj.onTouchBegin:Add(func)
end

function M:onTouchMove(func)
    self._obj.onTouchMove:Add(func)
end

function M:onTouchEnd(func)
    self._obj.onTouchEnd:Add(func)
end

---------- 拖拽 -----------
function M:onDragStart(func)
    self._obj.onDragStart:Set(func)
end

function M:onDragMove(func)
    self._obj.onDragMove:Add(func)
end

function M:onDragEnd(func)
    self._obj.onDragEnd:Add(func)
end

---------- enter - exit -----------
function M:onAddedToStage(func)
    self._obj.onAddedToStage:Add(func)
end
function M:setAddedToStage(func)
    self._obj.onAddedToStage:Set(func)
end
function M:onRemovedFromStage(func)
    self._obj.onRemovedFromStage:Add(func)
end
function M:setOnRemovedFromStage(func)
    self._obj.onRemovedFromStage:Set(func)
end

---------- 改变事件 -----------
function M:onSizeChanged(func)
    self._obj.onSizeChanged:Add(func)
end
function M:onPositionChanged(func)
    self._obj.onPositionChanged:Add(func)
end


-- 添加关联
function M:addRelation(...)
    self._obj:AddRelation(...)
end
-- 删除关联
function M:removeRelation(target, relationType)
    self._obj:RemoveRelation(target, relationType)
end


---------- 坐标转换 ----------
-- Transforms a point from the local coordinate system to global (Stage) coordinates.
function M:localToGlobal(vec2)
    return self._obj:LocalToGlobal(vec2)
end

-- Transforms a point from global (Stage) coordinates to the local coordinate system.
function M:globalToLocal(vec2)
    return self._obj:GlobalToLocal(vec2)
end

-- 如果要转换任意两个UI对象间的坐标，例如需要知道A(self)里面的坐标(10,10)在B(comp)里面的位置，可以用：
function M:transformPoint(vec2, comp)
    return self._obj:TransformPoint(vec2, comp:getObj())
end

function M:worldToLocal(Vector3, camera)
    return self._obj:WorldToLocal(Vector3, camera)
end


-- 透明度
function M:setAlpha(alpha)
    self._obj.alpha = alpha
end

function M:getAlpha()
    return self._obj.alpha
end

-- 缩放
function M:setScale(a, b)
    if not self._obj then
        return
    end
    if b then
        self._obj:SetScale(a, b)
    else
        self._obj:SetScale(a, a)
    end
end

--旋转
function M:setRotation(rotation)
    self._obj.rotation = rotation
end
function M:getRotation()
    return self._obj.rotation
end

function M:setScaleX(scaleX)
    self._obj.scaleX = scaleX
end

function M:getScale()
    return self._obj.scaleX, self._obj.scaleY
end

function M:getScaleX()
    return self._obj.scaleX
end

function M:setScaleY(scaleY)
    self._obj.scaleY = scaleY
end

function M:getScaleY()
    return self._obj.scaleY
end

function M:setSkew(x, y)
    self._obj.skew = Vector2(x, y)
end
function M:getSkew()
    return self._obj.skew
end

function M:dispose()
    if self._clickFunc then
        Cache.guideCache:removeUI(self)
    end

    if self._obj then
        self._obj:Dispose()
        self._obj = false
    end

    --进入对象池的GObject不会调该方法清除引用，在这边补上
    self:destroy()
end

function M:startDrag(id)
    self._obj:StartDrag(id)
end

function M:stopDrag()
    self._obj:StopDrag()
end

function M:setHome(obj)
    self._obj:SetHome(obj)
end

-- bug在addChild后setName不会改变父节点的children
function M:setName(name)
    if name == self._obj.name then
        return
    end
    Cache.guideCache:removeUI(self)
    self._obj.name = name or string.empty
    self:clearPath()
end

function M:getName()
    if self._obj then
        return self._obj.name
    end
    return ""
end

function M:setCustomSize(rect)
    self._obj.displayObject.customRect = rect
end

-- 获取parent
function M:hasParent()
    return self._obj.parent
end

function M:isDisposed()
    if self._obj == false then
        return true
    end
    return self._obj.isDisposed
end

function M:center(restraint)
    restraint = restraint or false
    self._obj:Center(restraint)
end

function M:invalidateBatchingState()
    self._obj:InvalidateBatchingState()
end

function M:getResourceURL()
    return self._obj.resourceURL
end


----------
-- 设置动作
function M:setTweenerHelper(tweener)
    self._tweenerHelper = tweener
end

function M:getTweenerHelper()
    return self._tweenerHelper
end

function M:stopAction()
    if self._tweenerHelper then
        self._tweenerHelper:stopAction()
    end
end
----------
--[[
public GTweener TweenMove(Vector2 endValue, float duration)
public GTweener TweenMoveX(float endValue, float duration)
public GTweener TweenMoveY(float endValue, float duration)
public GTweener TweenScale(Vector2 endValue, float duration)
public GTweener TweenScaleX(float endValue, float duration)
public GTweener TweenScaleY(float endValue, float duration)
public GTweener TweenResize(Vector2 endValue, float duration)
public GTweener TweenFade(float endValue, float duration)
public GTweener TweenRotate(float endValue, float duration)
]]
----------

function M:parabola(beginPos, endPos, height, duration, callback)
    local angle = 90
    local radian = angle * math.pi / 180.0
    local q2x = beginPos.x + (endPos.x - beginPos.x) / 2.0
    local midPos = Vector3(q2x, beginPos.y + math.cos(radian) * q2x - height, 0)

    local p1 = FairyGUI.GPathPoint(beginPos, midPos)
    local p2 = FairyGUI.GPathPoint(endPos, midPos)
    local path = FairyGUI.GPath()
    path:Create(p1, p2)

    local tweener = FairyGUI.GTween.To(0, 0, 0)
    tweener:SetDuration(duration)
    tweener:SetPath(path)
    tweener:SetTarget(self:getObj(), FairyGUI.TweenPropType.Position):SetEase(FairyGUI.EaseType.Linear)
    tweener:OnComplete(function()
        if callback then
            callback()
        end
    end)
end

function M:lineCompToComp(comp, vec2, duration, callback)
    local beginPos = self._obj.position
    local endPos = comp:transformPoint(vec2, self:getParent())
    endPos = Vector3(endPos.x, endPos.y, 0)

    local q2x = beginPos.x + (endPos.x - beginPos.x) / 2.0
    local q2y = beginPos.y + (endPos.y - beginPos.y) / 2.0
    local midPos = Vector3(q2x, q2y, 0)

    local p1 = FairyGUI.GPathPoint(beginPos, midPos)
    local p2 = FairyGUI.GPathPoint(endPos, midPos)
    local path = FairyGUI.GPath()
    path:Create(p1, p2)

    local tweener = FairyGUI.GTween.To(0, 0, 0)
    tweener:SetDuration(duration)
    tweener:SetPath(path)
    tweener:SetTarget(self._obj, FairyGUI.TweenPropType.Position):SetEase(FairyGUI.EaseType.Linear)
    tweener:OnComplete(function()
        if callback then
            callback()
        end
    end)
end

function M:lineToComp(comp, vec2, duration, callback)
    local beginPos = self._obj.position
    local endPos = comp:transformPoint(vec2, self:getParent())
    endPos = Vector3(endPos.x, endPos.y, 0)

    local q2x = beginPos.x + (endPos.x - beginPos.x) / 2.0
    local q2y = beginPos.y + (endPos.y - beginPos.y) / 2.0
    local midPos = Vector3(q2x, q2y, 0)

    local p1 = FairyGUI.GPathPoint(beginPos, midPos)
    local p2 = FairyGUI.GPathPoint(endPos, midPos)
    local path = FairyGUI.GPath()
    path:Create(p1, p2)

    local tweener = FairyGUI.GTween.To(0, 0, 0)
    tweener:SetDuration(duration)
    tweener:SetPath(path)
    tweener:SetTarget(self._obj, FairyGUI.TweenPropType.Position):SetEase(FairyGUI.EaseType.Linear)
    tweener:OnComplete(function()
        if callback then
            callback()
        end
    end)
end

function M:parabolaToComp(comp, vec2, height, duration, callback)
    local beginPos = self._obj.position
    local endPos = comp:transformPoint(vec2, self:getParent())
    endPos = Vector3(endPos.x, endPos.y, 0)

    local angle = 90
    local radian = angle * math.pi / 180.0
    local q2x = beginPos.x + (endPos.x - beginPos.x) / 2.0
    local midPos = Vector3(q2x, beginPos.y + math.cos(radian) * q2x - height, 0)

    local p1 = FairyGUI.GPathPoint(beginPos, midPos)
    local p2 = FairyGUI.GPathPoint(endPos, midPos)
    local path = FairyGUI.GPath()
    path:Create(p1, p2)

    local tweener = FairyGUI.GTween.To(0, 0, 0)
    tweener:SetDuration(duration)
    tweener:SetPath(path)
    tweener:SetTarget(self._obj, FairyGUI.TweenPropType.Position):SetEase(FairyGUI.EaseType.Linear)
    tweener:OnComplete(function()
        if callback then
            callback()
        end
    end)
end

rawset(_G, "GObject", M)
