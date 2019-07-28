using THEditor;
using THGame;
namespace STGEditor
{
    public static class PrefabBuildConfig
    {
        public static readonly string artSrcFolder = "Assets/ResEditor";
        public static readonly string srcResource = PathUtil.Combine(artSrcFolder, "Art");
        public static readonly string srcModels = PathUtil.Combine(srcResource,"Models");
        public static readonly string srcSprites = PathUtil.Combine(srcResource, "Sprites");
        public static readonly string srcPublicFx = PathUtil.Combine(srcResource, "Effects/Prefab/PublicFx");
        public static readonly string srcLevels = PathUtil.Combine(srcResource, "Levels");

        public static readonly string artTempFolder = "Assets/ResEditor/Z_AutoProcess";
        public static readonly string temResource = PathUtil.Combine(artTempFolder, "ResTemp");
        public static readonly string tempModels = PathUtil.Combine(temResource, "Models");
        public static readonly string tempSprites = PathUtil.Combine(temResource, "Sprites");
        public static readonly string tempPublicFx = PathUtil.Combine(temResource, "Effects");
        public static readonly string tempLevels = PathUtil.Combine(temResource, "Levels");

        public static readonly string temMd5 = PathUtil.Combine(temResource, "Md5");
        public static readonly string tempModelMd5s = PathUtil.Combine(temMd5, "Models");
        public static readonly string tempSpriteMd5s = PathUtil.Combine(temMd5, "Sprites");
        public static readonly string tempPublicFxMd5s = PathUtil.Combine(temMd5, "Effects");
        public static readonly string tempLevelMd5s = PathUtil.Combine(temMd5, "Levels");

    }
}

