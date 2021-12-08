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

return M