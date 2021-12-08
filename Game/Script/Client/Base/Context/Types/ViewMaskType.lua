local M = {
    None = 0, -- 无底
    Translucent = 1, -- 半透黑底
    Black = 2, -- 全遮盖纯黑底，可隐藏场景
    SceneBlur = 3, -- 场景虚化效果
}

rawset(_G, "ViewMaskType", false)
ViewMaskType = M