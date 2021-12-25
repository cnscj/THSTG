local M = class("CoroutineTest")
function M:ctor()
    -- /Users/cnscj/UnityWorkspace/THSTG/Game/Resource/pc/fgui/mainui
    -- UIPackageManager:loadPackage("/Users/cnscj/UnityWorkspace/THSTG/Engine/STGGame/Assets/GameAssets/FGUI/MainUI")

    Timer:scheduleOnce(1,function ( ... )
        UIManager:openView("MainUIView")
    end)
    Timer:scheduleOnce(3,function ( ... )
        UIManager:closeView("MainUIView")
    end)
    -- UIManager:closeView("MainUIView")
    -- print(15,CS.UnityEngine.Application.dataPath)
    -- print(15,CS.UnityEngine.Application.streamingAssetsPath)
    -- print(15,CS.UnityEngine.Application.persistentDataPath)
    -- print(15,CS.UnityEngine.Application.temporaryCachePath)
end

return M