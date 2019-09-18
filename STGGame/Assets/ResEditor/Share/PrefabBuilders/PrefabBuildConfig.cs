
using XLibrary;

namespace STGGame
{
    public static class PrefabBuildConfig
    {
        public static readonly string artSrcFolder = "Assets/ResEditor";

        public static readonly string srcArtResource = PathUtil.Combine(artSrcFolder, "Art");
        public static readonly string srcModels = PathUtil.Combine(srcArtResource, "Models");
        public static readonly string srcSprites = PathUtil.Combine(srcArtResource, "Sprites");
        public static readonly string srcPublicFx = PathUtil.Combine(srcArtResource, "Effects", "Prefab", "PublicFx");
        public static readonly string srcLevels = PathUtil.Combine(srcArtResource, "Levels");

        public static readonly string srcProgramResource = PathUtil.Combine(artSrcFolder, "Program");
        public static readonly string srcUIs = PathUtil.Combine(srcProgramResource, "UIs","Modules");

        public static readonly string srcDesignResource = PathUtil.Combine(artSrcFolder, "Design");
        public static readonly string srcConfigs = PathUtil.Combine(srcDesignResource, "Configs");

        ///
        public static readonly string artTempFolder = "Assets/GameAssets";
        public static readonly string temResource = PathUtil.Combine(artTempFolder, "");
        public static readonly string tempModels = PathUtil.Combine(temResource, "Models");
        public static readonly string tempSprites = PathUtil.Combine(temResource, "Sprites");
        public static readonly string tempPublicFx = PathUtil.Combine(temResource, "Effects");
        public static readonly string tempLevels = PathUtil.Combine(temResource, "Levels");
        public static readonly string tempUIs = PathUtil.Combine(temResource, "UIs");
        public static readonly string tempConfigs = PathUtil.Combine(temResource, "Configs");

        public static readonly string temMd5 = PathUtil.Combine(temResource, "_Md5");
        public static readonly string tempModelMd5s = PathUtil.Combine(temMd5, "Models");
        public static readonly string tempSpriteMd5s = PathUtil.Combine(temMd5, "Sprites");
        public static readonly string tempPublicFxMd5s = PathUtil.Combine(temMd5, "Effects");
        public static readonly string tempLevelMd5s = PathUtil.Combine(temMd5, "Levels");
        public static readonly string tempUIMd5s = PathUtil.Combine(temMd5, "UIs");
        public static readonly string tempConfigMd5s = PathUtil.Combine(temMd5, "Configs");
    }
}

