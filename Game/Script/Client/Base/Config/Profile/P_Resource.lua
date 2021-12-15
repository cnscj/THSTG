return 
{
    keyFunc = {
        ["{platform}"] = function ()
            return "pc"
        end,

    },
    fguiEditorPath = "/Volumes/Data/ProjectTest/FGUI",--"/Users/cnscj/UnityWorkspace/THSTG/Engine/STGGame/Assets/GameAssets/FGUI",
    resourcePath = "/Users/cnscj/UnityWorkspace/THSTG/Game/Resource",
    resDict = {
        fguiFolder = {abPattern = "fgui/{id}",resPattern = ""},
        modelFolder = {abPattern = "model/{id}.ab",resPattern = "Assets/GameAssets/Effect/{id}.prefab"},
        effectFolder = {abPattern = "effect/{id}.ab",resPattern = "Assets/GameAssets/Effect/{id}.prefab"},
    }


}