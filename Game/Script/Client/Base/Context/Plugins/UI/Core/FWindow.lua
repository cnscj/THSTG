local M = class("FWindow", FView)

function M:ctor(obj,args)
    --是否是非模态的(模态窗口必须完成当前窗口才能继续)
    self._isModeless = false

    -- [覆盖父类] mask类型，细节查看ViewMaskType
    self._maskType = ViewMaskType.Translucent

    -- [覆盖父类] 默认可按esc关闭窗口
    self._escClosable = true

    -- [待子类覆盖] 标题文本
    self._title = false

    -- 窗口框，如果没有frame组件，frame就是root自己
    self._frame = false
    ---@type GButton@关闭按钮
    self._closeBtn = false
    -- 窗口标题
    self._titleLabel = false
    --图片标题
    self._titleImageLoader = false
    -- 内容区域，用于添加界面加载菊花动画
    self._modalArea = false
    -- 内容节点
    self._contentNode = false
    -- 拖动区域
    self._dragArea = false
end

function M:init(obj,args)
    self._parentLayer = args and args.parentLayer or ViewLayer.Window or 0
end

function M:_initObj( ... )
    if self._obj then
        self._frame = self.root:getChild("frame")
        self._frame = self._frame or self

        --关闭按钮
        self._closeBtn = self._frame:getChild("closeBtn")
        if self._closeBtn then
            self._closeBtn:onClick(function ( ... )
                self:closeView()
            end)
        end

        self._titleImageLoader = self._frame:getChild("icon")

        self._titleLabel = self._frame:getChild("title")
        self:setTitle(self._title)

        -- 显示等待区域
        self._modalArea = self._frame:getChild("modalArea")

        -- 内容节点
        self._contentNode = self._frame:getChild("contentNode")

        -- 拖动区域
        if self._dragArea == false then
            self._dragArea = self._frame:getChild("dragArea")
            if self._dragArea then
                self._dragArea:setDraggable(true)
                self._dragArea:onDragStart(function (context)
                    context:PreventDefault()
                    self._root:startDrag(context.data)
                end)
            end
        end
    end

    --背景遮罩
    if self._maskType ~= ViewMaskType.None then
        if self._maskType == ViewMaskType.Translucent then

        elseif self._maskType == ViewMaskType.Black then

        elseif self._maskType == ViewMaskType.SceneBlur then

        end
    end

    self:super("FView", "_initObj", ...)
end

function M:setTitle( title )
    if self._titleLabel then
        if not string.isEmpty(title) then
            self._titleLabel:setText(title)
        end
    end
end

return M