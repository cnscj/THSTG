---@class GScrollPane
local M = class("GScrollPane")

function M:ctor(obj)
    self._obj = obj
    self._scrollFunc = false
end

--这2个函数要成对写
function M:init(obj)
end
function M:destroy()
    self._scrollFunc = false
    self._obj = false
end

function M:getObj()
    return self._obj
end

function M:getHeader()
    return self._obj.header
end

function M:getOwner()
    return self._obj.owner
end

function M:getFooter()
    return self._obj.footer
end

function M:lockHeader(size)
    return self._obj:LockHeader(size)
end

function M:lockFooter(size)
    return self._obj:LockFooter(size)
end

function M:scrollTop(...)
    self._obj:ScrollTop(...)
end

function M:scrollBottom(...)
    self._obj:ScrollBottom(...)
end

function M:scrollDown(...)
    self._obj:ScrollDown(...)
end

function M:scrollUp(...)
    self._obj:ScrollUp(...)
end

function M:scrollLeft(...)
    self._obj:ScrollLeft(...)
end

function M:scrollRight(...)
    self._obj:ScrollRight(...)
end

function M:isOnTop()
    return self._obj.percY <= 0
end

function M:isOnBottom()
    return self._obj.percY >= 1
end

function M:isOnRight()
    return self._obj.percX >= 1
end

function M:isOnLeft()
    return self._obj.percX <= 0
end

function M:onScroll(func)
    self._obj.onScroll:Add(func)
end

function M:onScrollEnd(func)
    self._obj.onScrollEnd:Add(func)
end

function M:onPullDownRelease(func)
    self._obj.onPullDownRelease:Add(func)
end

function M:onPullUpRelease(func)
    self._obj.onPullUpRelease:Add(func)
end

function M:getContentWidth()
    return self._obj.contentWidth
end

function M:getContentHeight()
    return self._obj.contentHeight
end

function M:getViewWidth()
    return self._obj.viewWidth
end

function M:getViewHeight()
    return self._obj.viewHeight
end

function M:setPosX(val, ani)
    self._obj:SetPosX(val, ani)
end

function M:setPosY(val, ani)
    self._obj:SetPosY(val, ani)
end

function M:getPosX()
    return self._obj.posX
end

function M:getPosY()
    return self._obj.posY
end

function M:setPercX(val, ani)
    self._obj:SetPercX(val, ani)
end

function M:setPercY(val, ani)
    self._obj:SetPercY(val, ani)
end

function M:getPercX()
    return self._obj.percX
end

function M:getPercY()
    return self._obj.percY
end

function M:isBottomMost()
    return self._obj.isBottomMost
end

function M:isRightMost()
    return self._obj.isRightMost
end

-- 设置按钮显示回调， 默认为竖直的 ,arrow1右、下， arrow2左、上
function M:setScrollArrow(arrow1, arrow2, isHorizontal)
    self._scrollFunc = function()
        if isHorizontal then
            if arrow1 then
                if self:isOnRight() then
                    arrow1:setVisible(false)
                else
                    if self:getContentWidth() > self:getViewWidth() then
                        arrow1:setVisible(true)
                    else
                        arrow1:setVisible(false)
                    end
                end
            end
            if arrow2 then
                if self:isOnLeft() then
                    arrow2:setVisible(false)
                else
                    if self:getContentWidth() > self:getViewWidth() then
                        arrow2:setVisible(true)
                    else
                        arrow2:setVisible(false)
                    end
                end
            end
        else
            -- 竖直方向的
            if arrow1 then
                if self:isOnBottom() then
                    arrow1:setVisible(false)
                else
                    if self:getContentHeight() > self:getViewHeight() then
                        arrow1:setVisible(true)
                    else
                        arrow1:setVisible(false)
                    end
                end
            end

            if arrow2 then
                if self:isOnTop() then
                    arrow2:setVisible(false)
                else
                    if self:getContentHeight() > self:getViewHeight() then
                        arrow2:setVisible(true)
                    else
                        arrow2:setVisible(false)
                    end
                end
            end
        end
    end
    self:onScroll(self._scrollFunc)
    self._obj.sizeChanged = self._scrollFunc

    if arrow1 then
        arrow1:setVisible(false)
    end
    if arrow2 then
        arrow2:setVisible(false)
    end
end

function M:setTouchEffect(bol)
    self._obj.touchEffect = bol
end

function M.setScrollStep(step)
    self._obj.scrollStep = step
end

function M:getScrollStep()
    return self._obj.scrollStep
end
function M:scrollToView(gobject, anim, setFirst)
    self._obj:ScrollToView(gobject, anim, setFirst)
end
function M:scrollToRect(rect, anim, setFirst)
    self._obj:ScrollToView(rect, anim, setFirst)
end

---获得水平滚动条
function M:getHzScrollBar()
    return self._obj.hzScrollBar
end

---获得垂直滚动条
function M:getVtScrollBar()
    return self._obj.vtScrollBar
end

rawset(_G, "GScrollPane", M)

--[[
http://www.fairygui.com/guide/editor/scrollpane.html#ScrollPane

ScrollPane

当组件的“溢出处理”设置为“滚动”后，可以通过GComponent.scrollPane使用滚动相关的功能，例如：
ScrollPane scrollPane =  aComponent.scrollPane;
//设置滚动位置为100像素
scrollPane.posX = 100;
//滚动到中间位置，带动画过程
scrollPane.SetPercX(0.5f, true);
当你增删子组件后，或者移动子组件的位置、调整子组件的大小，容器是自动更新滚动区域的，不需要调用任何API。这个刷新发生在本帧绘制之前。如果你希望立刻访问子元件的正确坐标，那么可以调用EnsureBoundsCorrect通知GComponent立刻重排。EnsureBoundsCorrect是一个友好的函数，你不用担心重复调用会有额外性能消耗。
ScrollPane中常用的API有：
viewWidth viewHeight 视口宽度和高度。
contentWidth contentHeight 内容高度和宽度。
percX percY SetPercX SetPercY 获得或设置滚动的位置，以百分比来计算，取值范围是0-1。如果希望滚动条从当前值到设置值有一个动态变化的过程，可以使用Set方法，它们提供了一个是否使用缓动的参数。
posX posY SetPosX SetPosY 获得或设置滚动的位置，以绝对像素值来计算。取值范围是0-最大滚动距离。垂直最大滚动距离=（内容高度-视口高度），水平最大滚动距离=（内容宽度-视口宽度）。如果希望滚动条从当前值到设置值有一个动态变化的过程，可以使用Set方法，它们提供了一个是否使用缓动的参数。
currentPageX currentPageY setCurrentPageX setCurrentPageY 如果滚动设置为页面模式，那么可以通过这些方法设置或者获得当前的页面索引。如果要获得页面数量，可以用contentWidth/viewWidth或者contentHeight/viewHeight。
ScrollLeft ScrollRight ScrollUp ScrollDown 向指定方向滚动N*scrollStep。例如，如果scrollStep=20，那么ScrollLeft(1)表示向左滚动20像素，ScrollLeft(2)表示向左滚动40像素。注意：如果滚动属性设置了贴近元件，例如元件大小为41像素，则需要滚动距离超过20像素，才能真正发生滚动，那么如果调用ScrollLeft(1)，在scrollStep=20的情况下，会导致看不到任何效果。
如果滚动设置为页面模式，那这几个API也有“翻一页”的作用。
ScrollToView 调整滚动位置，使指定的元件出现在视口内。
touchEffect 打开或关闭触摸滚动功能。关闭触摸滚动后，用户就不能拖拽视口进行滚动了。
scrollStep 这个值是指滚动“一格”的距离。这个距离有三个用途：a）scrollUp/scrollDown/scrollLeft/scrollRight； b）点击滚动条的箭头按钮； c）鼠标滚轮，鼠标滚轮滚一次的距离是scrollStep*2。
bounceBackEffect 可以打开或关闭边缘回弹功能。
mouseWheelEnabled 打开或关闭鼠标滚动支持。
decelerationRate 减速率，调整这个值可以控制惯性滚动的距离和时间。惯性滚动是指手指拖动一定距离然后释放后，滚动容器内容继续滚动一定距离后停止。越接近1，减速越慢，意味着滑动的时间和距离更长。默认值是UIConfig.defaultScrollDecelerationRate。
CancelDragging 当滚动面板处于拖拽滚动状态或即将进入拖拽状态时，可以调用此方法停止或禁止本次拖拽。
可以侦听滚动改变，在任何情况下滚动位置改变都会触发这个事件。
//Unity/Cry
scrollPane.onScroll.Add(onScroll);
//AS3
scrollPane.addEventListener(Event.SCROLL, onScroll);
//Egret
scrollPane.addEventListener(ScrollPane.SCROLL, this.onScroll, this);
//Laya，注意是用组件侦听，不是ScrollPane
aComponent.on(fairygui.Events.SCROLL, this, this.onScroll);
//Cocos2dx，注意是用组件侦听，不是ScrollPane
aComponent->addEventListener(UIEventType::Scroll, CC_CALLBACK_1(AClass::onScroll, this));
和滚动相关的事件还有：
ScrollEnd 惯性滚动结束后回调。
PullDownRelease 下拉刷新回调。
PullUpRelease 上拉刷新回调。
//Unity/Cry
scrollPane.onScrollEnd.Add(onScrollEnd);
scrollPane.onPullDownRelease.Add(onPullDownRelease);
scrollPane.onPullUpRelease.Add(onPullUpRelease);
]]

--[[
/// <summary>
/// 滚动到达边缘时是否允许回弹效果。
/// </summary>
public bool bouncebackEffect
{
	get { return _bouncebackEffect; }
	set { _bouncebackEffect = value; }
}

/// <summary>
/// 是否允许拖拽内容区域进行滚动。
/// </summary>
public bool touchEffect
{
	get { return _touchEffect; }
	set { _touchEffect = value; }
}

/// <summary>
/// 是否允许惯性滚动。
/// </summary>
public bool inertiaDisabled
{
	get { return _inertiaDisabled; }
	set { _inertiaDisabled = value; }
}

/// <summary>
/// 是否允许在左/上边缘显示虚化效果。
/// </summary>
public bool softnessOnTopOrLeftSide
{
	get { return _softnessOnTopOrLeftSide; }
	set { _softnessOnTopOrLeftSide = value; }
}

/// <summary>
/// 滚动位置是否保持贴近在某个元件的边缘。
/// </summary>
public bool snapToItem
{
    get { return _snapToItem; }
    set { _snapToItem = value; }
}
]]
