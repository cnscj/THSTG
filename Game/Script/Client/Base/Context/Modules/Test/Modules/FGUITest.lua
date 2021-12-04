local M = class("CoroutineTest")
function M:ctor()
    -- /Users/cnscj/UnityWorkspace/THSTG/Game/Resource/pc/fgui/mainui
    -- UIPackageManager:loadPackage("/Users/cnscj/UnityWorkspace/THSTG/Engine/STGGame/Assets/GameAssets/FGUI/MainUI")
    UIManager:openView("MainUIView")
end

return M