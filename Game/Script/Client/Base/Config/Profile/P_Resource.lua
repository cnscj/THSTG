return 
{
    --TODO:
    keyFunc = {
        ["{platform}"] = function ()
            return "pc"
        end,

    },
    fguiEditorPath = "/Volumes/Data/ProjectTest/FGUI",--"/Users/cnscj/UnityWorkspace/THSTG/Engine/STGGame/Assets/GameAssets/FGUI",
    resourcePath = "/Users/cnscj/UnityWorkspace/THSTG/Game/Resource",
    manifestPattern = "{platform}/{platform}.manifest",
    resDict = {
        modelFolder = {abPattern = "{platform}/model/{id}.ab",resPattern = "Assets/GameAssets/Effect/{id}.prefab"},
        effectFolder = {abPattern = "{platform}/effect/{id}.ab",resPattern = "Assets/GameAssets/Effect/{id}.prefab"},
    }


}