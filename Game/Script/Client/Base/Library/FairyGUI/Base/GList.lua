---@class GList:GComponent
local M = class("GList", GComponent)

function M:ctor(obj)
    self._dataProvider = {}
    self._dataTemplate = {}
    self._class = GComponent
    self._classArgs = false
    self._alreadySetData = false
    self._stencil = false
end

function M:init(obj)
end
function M:destroy()
    self._dataProvider = false

    if self._dataTemplate then
        for k, v in pairs(self._dataTemplate) do
            if v.destroy then
                v:destroy()
            end
        end
        self._dataTemplate = false
    end

    self:super("GComponent", "destroy")
end

-- 设置虚拟列表
function M:setVirtual()
    self._obj:SetVirtual()
end

-- 默认false，如果已经选中，再次点击不会触发onClickItem
function M:setForce(force)
    self._obj.force = force
end


-- 循环列表只支持单行或者单列的布局，不支持流动布局和分页布局。
function M:setLoop()
    self._obj:SetVirtualAndLoop()
end

-- 当点击某个item时，如果这个item处于部分显示状态，那么列表将会自动滚动到整个item显示完整。
function M:scrollItemToViewOnClick(bool)
    self._obj.scrollItemToViewOnClick = bool
end

-- 自动大小
function M:setAutoResizeItem(bool)
    self._obj.autoResizeItem = bool
end

--------------------------------------------------
function M:onClickItem(func)
    -- function (context)
    self._obj.onClickItem:Set(func)
end

function M:addClickItem(func)
    -- function (context)
    self._obj.onClickItem:Add(func)
end

function M:removeClickItem(func)
    self._obj.onClickItem:Remove(func)
end

-- 原生itemRenderer
function M:itemRenderer(func)
    -- function (index, obj)
    self._obj.itemRenderer = func
end

-- 设置多样式虚拟列表
function M:itemProvider(func)
    -- function (index)
    self._obj.itemProvider = function(index)
        return func(self._dataProvider[index + 1], index + 1)
    end
end

function M:resetCallback(func)
    self._obj.itemResetCallback = function(obj)
        local comp = self._dataTemplate[obj]
        if comp then
            func(comp)
        end
    end
end

function M:setClass(clsType, args)
    local cls = GComponent
    if (type(clsType) == "string") then
        cls = FGUIUtil.getCompByType(clsType)
    elseif (type(clsType) == "table") then
        cls = clsType
    end
    self._class = cls
    if args ~= nil then
        self._classArgs = args
    end
end

---@alias GlistHandler fun(data: any,index:number,comp:GComponent,obj:any):void


---@param func GlistHandler
function M:setState(func)
    self._obj.itemRenderer = function(index, obj)
        local comp = self._dataTemplate[obj]
        if comp == nil then
            comp = self._class.new(obj, self._classArgs, false, self._view)
            self._dataTemplate[obj] = comp
        end
        -- function (data, index, comp, obj)
        local luaIndex = index + 1
        local data = self:getDataProvider()[luaIndex]
        if type(data) == "table" and data.key then
            comp:setPath(self:getListPath(data.key))
        else
            comp:setPath(self:getListPath(luaIndex))
        end
        Cache.guideCache:saveUI(comp, nil, true)
        func(self._dataProvider[luaIndex], luaIndex, comp, obj, self)
    end
end

function M:getListPath(index)
    if not index then
        return self:getPath()
    end
    return self:getPath() .. "/" .. index
end

function M:getDataTemplate()
    return self._dataTemplate
end

--function M:disposeRedDot()
--    if self._dataTemplate then
--        for i, v in pairs(self._dataTemplate) do
--            if v.disposeRedDot then
--                v:disposeRedDot()
--            end
--        end
--    end
--end

function M:setDataProvider(array)
    if array then
        self._dataProvider = array
    else
        self._dataProvider = {}
    end
    if __DEBUG__ then
        if self._obj.numItems > 0 and self._alreadySetData == false then
            local str = getTraceback()
            -- if not Cache.testCache:updateTracebackLabel(str) then
            printWarning("list 需勾选：发布时自动清空！！！相对应功能同学看看！！！")
            log(str)
            -- end
        end
        self._alreadySetData = true
    end
    self:setNumItems(#self._dataProvider)
end

function M:getDataProvider()
    return self._dataProvider
end

-- 刷新列表
function M:refreshVirtualList(isImmediate)
    if not isImmediate then
        self._obj:RefreshVirtualList()
    else
        self._obj:RefreshVirtualListImmediate()
    end
end

-- 重新设置长度大小
function M:resizeToFit(count)
    if count == nil then
        count = self._obj.numItems
    end
    return self._obj:ResizeToFit(count)
end

-- list里面的item宽高会动态变化的，用这个
function M:resizeToFitEx(count, exValue)
    exValue = exValue or 0
    -- 要马上刷新，不然子的数量不一定是对的
    if self._obj.isVirtual then
        self:resizeToFit()
        self:refreshVirtualList(true)
    end

    local layout = self._obj.layout
    local isColumn = layout == FairyGUI.ListLayoutType.SingleColumn or layout == FairyGUI.ListLayoutType.FlowHorizontal
    if isColumn then
        local res = self:getFixHeight(count)
        self:setHeight(self:getFixHeight(count) + exValue)
    else
        local res = self:getFixWidth(count)
        self:setWidth(self:getFixWidth(count) + exValue)
    end
end

--非虚拟列表的刷新
function M:refreshList()
    self:setNumItems(#self._dataProvider)
end

--------------------------------------------------

-- 移动scroll
function M:scrollToView(index, action, setFirst)
    if action == nil then
        action = false
    end
    if setFirst == nil then
        setFirst = false
    end
    if index < 1 then
        index = 1
    end
    self._obj:ScrollToView(index - 1, action, setFirst)
end

function M:scrollToTop(action, setFirst)
    self:scrollToView(1, action, setFirst)
end

function M:scrollToBottom(action, setFirst)
    self:scrollToView(#self._dataProvider, action, setFirst)
end


-- 设置数量
function M:setNumItems(num)
    self._obj.numItems = num
end
function M:getNumItems()
    return self._obj.numItems
end

-- 获取当前选中的data
function M:getSelectedData()
    -- lua从1开始，底层是从0开始
    local index = self:getSelectedIndex()
    if index then
        return self._dataProvider[index]
    end
end

-- 获取当前选中第几个，list里面的item是单选按钮才生效
function M:getSelectedIndex()
    -- lua从1开始，底层是从0开始
    return self._obj.selectedIndex + 1
end
---@return GComponent
function M:getSelectedComp()
    return self:getCompByIndex(self:getSelectedIndex())
end

---获取视野中的第一个子物体下标
function M:getFirstChildInView()
    if not self._obj then
        return 0
    end
    return self._obj:GetFirstChildInView() + 1
end

-- //转换项目索引为显示对象索引。
-- int childIndex = aList.ItemIndexToChildIndex(1);
-- //转换显示对象索引为项目索引。
-- int itemIndex = aList.ChildIndexToItemIndex(1);
function M:setSelectedKey(key, click)
    local index = 1
    for k, v in ipairs(self._dataProvider) do
        if v.key == key then
            index = k
        end
    end
    self:setSelectedIndex(index, click)
end

function M:getSelectedKey()
    local index = self:getSelectedIndex()
    return self._dataProvider and self._dataProvider[index] and self._dataProvider[index].key
end

-- 选中第index个，并且触发点击事件，list里面的item是单选按钮才生效
function M:setSelectedIndex(index, click, setFirst)
    -- lua从1开始，底层是从0开始
    -- self._obj.selectedIndex = index - 1

    if self._obj.numChildren == 0 then
        return
    end

    local realIndex = index - 1
    if click then
        -- 先判断是否在显示列表
        -- 非虚拟列表，肯定是在显示中
        local childIndex = self._obj:ItemIndexToChildIndex(realIndex)
        if childIndex >= 0 and childIndex < self._obj.numChildren - 1 then
            -- 如果index在显示列表中，则不需要scrollItToView，最后再点击
            self._obj:AddSelection(realIndex, false)
            self._obj.onClickItem:Call(self._obj:GetChildAt(childIndex))
        else
            -- 如果index不在显示列表中，则需要scrollItToView，才能找对显示对象，最后再点击
            self._obj:AddSelection(realIndex, true)
            local curIndex = self._obj:ItemIndexToChildIndex(realIndex)
            self._obj.onClickItem:Call(self._obj:GetChildAt(curIndex))
        end
    else
        self._obj:AddSelection(realIndex, false)
    end
end

-- 选择指定index位置的item，并且将其置于list的第一显示位
function M:setSelectedIndexToFirst(index, click)
    -- lua从1开始，底层是从0开始
    -- self._obj.selectedIndex = index - 1
    if self._obj.numChildren == 0 then
        return
    end
    local realIndex = index - 1
    self._obj:ScrollToView(realIndex, false, true)
    self._obj:AddSelection(realIndex, false)
    if click then
        local curIndex = self._obj:ItemIndexToChildIndex(realIndex)
        self._obj.onClickItem:Call(self._obj:GetChildAt(curIndex))
    end
end

function M:addSelection(index, scrollItToView)
    if scrollItToView == nil then
        scrollItToView = false
    end
    self._obj:AddSelection(index - 1, scrollItToView)
end

-- 获取全部选中
function M:getSelection()
    return self._obj:GetSelection()
end

-- 取消某个选中
function M:removeSelection(index)
    self._obj:RemoveSelection(index - 1)
end

-- 取消全部选择
function M:clearSelection()
    return self._obj:ClearSelection()
end

-- 反选
function M:selectReverse()
    return self._obj:SelectReverse()
end

function M:selectAll()
    self._obj:SelectAll()
end

function M:forceRefreshSelectedItem(index)
    local selectedIndex
    if index then
        selectedIndex = index - 1
    else
        selectedIndex = self._obj.selectedIndex
    end
    local childIndex = self._obj:ItemIndexToChildIndex(selectedIndex)
    if childIndex >= 0 and childIndex < self._obj.numChildren then
        local item = self._obj:GetChildAt(childIndex)
        self._obj.itemRenderer(selectedIndex, item)
    end
end

--------------------------------------------------
function M:onPullDownRelease(func)
    self._obj.scrollPane.onPullDownRelease:Add(func)
end

function M:onPullUpRelease(func)
    self._obj.scrollPane.onPullUpRelease:Add(func)
end

function M:getHeader()
    return self._obj.scrollPane.header
end

function M:getScrollStep()
    return self._obj.scrollPane.scrollStep
end

function M:getFooter()
    return self._obj.scrollPane.footer
end

function M:itemIndexToChildIndex(index)
    -- lua从1开始，底层是从0开始
    return self._obj:ItemIndexToChildIndex(index - 1) + 1
end
function M:childIndexToItemIndex(index)
    -- lua从1开始，底层是从0开始
    return self._obj:ChildIndexToItemIndex(index - 1) + 1
end

--[[
    FairyGUI.ListSelectionMode.None
    FairyGUI.ListSelectionMode.Single
    FairyGUI.ListSelectionMode.Multiple
    FairyGUI.ListSelectionMode.Multiple_SingleClick
]]
function M:setSelectionMode(mode)
    self._obj.selectionMode = mode
end

function M:getSelectionMode()
    return self._obj.selectionMode
end

function M:getSelectedNode(index)
    index = index or self:getSelectedIndex()
    local childIndex = self._obj:ItemIndexToChildIndex(index - 1)
    if childIndex >= 0 and childIndex < self._obj.numChildren then
        local item = self._obj:GetChildAt(childIndex)
        return item
    end
end

function M:getSelectionComp(index)
    local item = self:getSelectedNode(index)
    return self._dataTemplate[item]
end

---@return GComponent@获取某个索引的组件
---@return GComponent
function M:getCompByIndex(index)
    local childIndex = self._obj:ItemIndexToChildIndex(index - 1)
    if childIndex >= 0 and childIndex < self._obj.numChildren then
        local obj = self._obj:GetChildAt(childIndex)
        return self._dataTemplate[obj]
    end
end

function M:isChildInView(index)
    local childIndex = self._obj:ItemIndexToChildIndex(index - 1)
    if childIndex >= 0 and childIndex < self._obj.numChildren then
        local obj = self._obj:GetChildAt(childIndex)
        return self._obj.scrollPane:IsChildInView(obj)
    end
    return false
end

-- 返回当前滚动位置是否在最下边
function M:isBottomMost()
    return self._obj.scrollPane.isBottomMost
end

function M:isRightMost()
    return self._obj.scrollPane.isRightMost
end

-- 在滚动结束时派发该事件。
function M:onScrollEnd(func)
    self._obj.scrollPane.onScrollEnd:Add(func)
end

function M:onScroll(func)
    self._obj.scrollPane.onScroll:Add(func)
end

function M:removeOnScroll(func)
    self._obj.scrollPane.onScroll:Remove(func)
end

function M:setColumnCount(count)
    self._obj.columnCount = count
end

function M:setLineCount(count)
    self._obj.lineCount = count
end

function M:getColumnGap()
    return self._obj.columnGap
end

function M:setColumnGap(gap)
    self._obj.columnGap = gap
end

function M:getLineGap()
    return self._obj.lineGap
end

function M:getNumChildren()
    return self._obj.numChildren
end

function M:getViewWidth()
    return self._obj.viewWidth
end

function M:getViewHeight()
    return self._obj.viewHeight
end

function M:removeChildrenToPool()
    self._obj:RemoveChildrenToPool()
end

function M:setMouseWheelEnabled(isEnabled)
    self._obj.scrollPane.mouseWheelEnabled = isEnabled
end

----- 自增接口 -----
--[[
如果list要用到控制器，而且要调用setStencil，这个时候会出问题，因为节点已经改变了。
这个时候吧这个list放到一个高级组里面，把控制器对到这个组。
]]
-- 加一层遮罩，只能做一次
function M:setStencil()
    -- self._obj:SetVirtual()
    -- 弄一个组件和list一样大
    -- 弄一个shape当做遮罩
    -- 把这个list放入这个组件
    -- XXX:如果List套了组的话,被控制器显隐会失效:所以建议不要用组,用组件
    if self._stencil == true then
        return
    end
    self._stencil = true

    -- local group = self._obj.grop
    local component = UI.newComponent({
        xy = self:getXY(),
        -- size = self._obj.size,
    })

    local margin = self:getObj().margin
    local graph = UI.newGraph({
        xy = Vector2(margin.left, margin.top),
        size = Vector2((self._obj.size.x- margin.left - margin.right) * self:getScaleX() , (self._obj.size.y - margin.top - margin.bottom) * self:getScaleY() ),
        rect = {
            lineSize = 1,
            lineColor = Color("#000000"),
            fillColor = Color("#000000"),
        },
        pivot = self:getPivot(),
        pivotAsAnchor = self:isPivotAsAnchor(),
    })

    -- 用组来做，然后切换
    component._obj.group = self._obj.group
    -- component._obj.Controllers = self._obj.Controllers

    -- local cnt = self._obj.parent.Controllers.Count
    -- for i = 1, cnt do
    --     local k = i - 1
    --     local c = self._obj.parent:GetControllerAt(k)
    --     component._obj:AddController(c)
    -- end
    -- local cnt = self._obj.parent.Controllers.Count
    -- for i = 1, cnt do
    --     local k = i - 1
    --     local c = self._obj.parent:GetControllerAt(k)
    --     c:Dispose()
    -- end


    local parent = self:getParent()
    local index = parent:getChildIndex(self:getObj())
    self:removeFromParent()
    self:setXY(0, 0)

    parent:addChild(component)
    component:addChild(graph)
    component:addChild(self)
    parent:setChildIndex(component:getObj(), index)
    
    graph:addRelation(self._obj, FairyGUI.RelationType.Size)
    graph:addRelation(self._obj, FairyGUI.RelationType.Center_Center)
    graph:addRelation(self._obj, FairyGUI.RelationType.Middle_Middle)
    component._obj.mask = graph._obj.displayObject

    return component
end

-- 设置按钮显示回调, 默认为竖直的 , arrow1右、下， arrow2左、上
function M:setScrollArrow(arrow1, arrow2, isHorizontal)
    local scrollPane = self:getScrollPane()
    scrollPane:setScrollArrow(arrow1, arrow2, isHorizontal)
end

function M:setListItemCondition(func)
    self._obj.listItemCondition = func
end

function M:getIndexByKey(key)
    local index = false
    local dataProvider = self:getDataProvider()
    for k, v in ipairs(dataProvider) do
        if v.key == key then
            index = k
            break
        end
    end
    return index
end

function M:setTouchable(enable)
    --fix 不滑动的时候没有喔
    if self._obj.scrollPane then
        self._obj.scrollPane.touchEffect = enable
    else
        self._obj.touchable = enable
    end
end

-- 设置水平对齐方式，AlignType(Left, Center, Right)
function M:setAlign(value)
    self._obj.align = value
end

-- 设置垂直对齐方式，VertAlignType(Top, Middle, Bottom)
function M:setVerticalAlign(value)
    self._obj.verticalAlign = value
end

function M:setPercX(value, ani)
    self._obj.scrollPane:SetPercX(value, ani)
end

function M:setPercY(value, ani)
    self._obj.scrollPane:SetPercY(value, ani)
end

function M:getPercX()
    return self._obj.scrollPane.percX
end

function M:getPercY()
    return self._obj.scrollPane.percY
end
--设置列距
function M:setColumnGap(gap)
    self._obj.columnGap = gap
end
--列表的进入动画
--[[
    NOTE:处于隐藏状态下的List(包括被控制器隐藏,visible为false),如果在enter处执行下面函数,可能会导致列表拖动失效
]]
---@param isHorizontal boolean  是否是水平方向的列表
function M:playEnterTrans(isHorizontal,firstIdx, setFirst)
    --从可视的第一个开始播动效,播完可见的就行了了，播放的时候禁用列表的触摸
    self:setTouchable(false)
    if not firstIdx then
        self:scrollToTop(nil, setFirst)
    else
        self:scrollToView(firstIdx, nil, setFirst)
    end
    local childNum = self:getNumChildren()
    local firstIndex = self:getFirstChildInView()
    local duration = 0.25
    local endIndex = firstIndex + childNum - 1
    if not firstIdx then
        endIndex = childNum
    end
    for i = firstIndex, endIndex do
        local comp = self:getCompByIndex(i)
        if comp then
            comp:setAlpha(0)
            local beginPos = false
            local delay = 0.1 + (i - firstIndex - 1) * 0.06
            if isHorizontal then
                beginPos = comp:getX()
                comp:setX(beginPos + 40 + i * 10)
            else
                beginPos = comp:getY()
                comp:setY(beginPos + 40 + i * 10)
            end
            comp:getObj():TweenFade(1, duration):SetEase(FairyGUI.EaseType.QuadOut):SetDelay(delay)
            local tween = false
            if isHorizontal then
                tween = comp:getObj():TweenMoveX(beginPos, duration):SetEase(FairyGUI.EaseType.QuadOut):SetDelay(delay)
            else
                tween = comp:getObj():TweenMoveY(beginPos, duration):SetEase(FairyGUI.EaseType.QuadOut):SetDelay(delay)
            end
            if i == endIndex then
                tween:OnComplete(function()
                    self:setTouchable(true)
                end)
            end
        end
        
    end
end


--[[
-- 停止传递下一层
context:StopPropagation()
]]

rawset(_G, "GList", M)
GList = M