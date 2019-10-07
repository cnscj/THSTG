using XLibrary;

namespace STGGame
{
    public static class AssetBuilderConfig
    {
        public static readonly string artSourceFolder = "Assets/AssetsEditor/";

        public static readonly string srcProgramResource = PathUtil.Combine(artSourceFolder, "Program");
        public static readonly string srcShaders = PathUtil.Combine(srcProgramResource, "Shaders");


        public static readonly string artTempFolder = "Assets/GameAssets";
        public static readonly string temResource = PathUtil.Combine(artTempFolder, "");
        public static readonly string tempModels = PathUtil.Combine(temResource, "Models");
        public static readonly string tempSprites = PathUtil.Combine(temResource, "Sprites");
        public static readonly string tempPublicFx = PathUtil.Combine(temResource, "Effects");
        public static readonly string tempLevels = PathUtil.Combine(temResource, "Levels");
        public static readonly string tempUIs = PathUtil.Combine(temResource, "UIs");
        public static readonly string tempConfigs = PathUtil.Combine(temResource, "Configs");
        public static readonly string tempCustoms = PathUtil.Combine(temResource, "Customs");

        public static readonly string bundleNameShaders = "shaders/{0}.ab";
        public static readonly string bundleNameLevels = "levels/{0}.ab";
        public static readonly string bundleNameEffects = "effects/{0}.ab";
        public static readonly string bundleNameSprites = "sprites/{0}.ab";
        public static readonly string bundleNameUIs = "uis/{0}.ab";
        public static readonly string bundleNameConfigs = "configs/{0}.ab";
        public static readonly string bundleNameCustoms = "customs/{0}.ab";
    }
}
