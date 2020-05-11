using XLibrary;

namespace STGGame
{
    public static class AssetBuilderConfig
    {
        public static readonly string artSourceFolder = "Assets/MainEditor/";

        public static readonly string srcProgramResource = XPathTools.Combine(artSourceFolder, "Program");
        public static readonly string srcShaders = XPathTools.Combine(srcProgramResource, "Shaders");


        public static readonly string artTempFolder = "Assets/GameAssets";
        public static readonly string temResource = XPathTools.Combine(artTempFolder, "");
        public static readonly string tempModels = XPathTools.Combine(temResource, "Models");
        public static readonly string tempSprites = XPathTools.Combine(temResource, "Sprites");
        public static readonly string tempPublicFx = XPathTools.Combine(temResource, "Effects");
        public static readonly string tempLevels = XPathTools.Combine(temResource, "Levels");
        public static readonly string tempUIs = XPathTools.Combine(temResource, "UIs");
        public static readonly string tempConfigs = XPathTools.Combine(temResource, "Configs");
        public static readonly string tempCustoms = XPathTools.Combine(temResource, "Customs");

        public static readonly string bundleNameShaders = "shaders/{0}.ab";
        public static readonly string bundleNameLevels = "levels/{0}.ab";
        public static readonly string bundleNameEffects = "effects/{0}.ab";
        public static readonly string bundleNameSprites = "sprites/{0}.ab";
        public static readonly string bundleNameUIs = "uis/{0}.ab";
        public static readonly string bundleNameConfigs = "configs/{0}.ab";
        public static readonly string bundleNameCustoms = "customs/{0}.ab";
    }
}
