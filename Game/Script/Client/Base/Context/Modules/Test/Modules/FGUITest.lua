local M = class("CoroutineTest")
function M:ctor()
    -- /Users/cnscj/UnityWorkspace/THSTG/Game/Resource/pc/fgui/mainui
    -- UIPackageManager:loadPackage("/Users/cnscj/UnityWorkspace/THSTG/Engine/STGGame/Assets/GameAssets/FGUI/MainUI")
    UIManager:openView("MainUIView")

    -- print(15,CS.UnityEngine.Application.dataPath)
    -- print(15,CS.UnityEngine.Application.streamingAssetsPath)
    -- print(15,CS.UnityEngine.Application.persistentDataPath)
    -- print(15,CS.UnityEngine.Application.temporaryCachePath)
end

return M