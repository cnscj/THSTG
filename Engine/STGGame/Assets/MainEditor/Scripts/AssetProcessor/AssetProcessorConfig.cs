
using XLibrary;

namespace STGGame
{
    public static class AssetProcessorConfig
    {
        public static readonly string artSrcFolder = "Assets/MainEditor";

        public static readonly string srcArtResource = XPathTools.Combine(artSrcFolder, "Art");
        public static readonly string srcModels = XPathTools.Combine(srcArtResource, "Models");
        public static readonly string srcSprites = XPathTools.Combine(srcArtResource, "Sprites");
        public static readonly string srcEffect = XPathTools.Combine(srcArtResource, "Effects", "Prefab");
        public static readonly string srcPublicFx = XPathTools.Combine(srcEffect, "PublicFx");
        public static readonly string srcSkillFx = XPathTools.Combine(srcEffect, "SkillFx");
        public static readonly string srcModelFx = XPathTools.Combine(srcEffect, "ModelFx");
        public static readonly string srcUIFx = XPathTools.Combine(srcEffect, "UIFx");
        public static readonly string srcSceneFx = XPathTools.Combine(srcEffect, "SceneFx");

        public static readonly string srcLevels = XPathTools.Combine(srcArtResource, "Levels");

        public static readonly string srcProgramResource = XPathTools.Combine(artSrcFolder, "Program");
        public static readonly string srcUIs = XPathTools.Combine(srcProgramResource, "UIs");
        public static readonly string srcScripts = XPathTools.Combine(srcProgramResource, "Scripts");

        public static readonly string srcDesignResource = XPathTools.Combine(artSrcFolder, "Design");
        public static readonly string srcConfigs = XPathTools.Combine(srcDesignResource, "Configs");

        ///
        public static readonly string artTempFolder = "Assets/GameAssets";
        public static readonly string temResource = XPathTools.Combine(artTempFolder, "");
        public static readonly string tempModels = XPathTools.Combine(temResource, "Models");
        public static readonly string tempSprites = XPathTools.Combine(temResource, "Sprites");
        public static readonly string tempEffect = XPathTools.Combine(temResource, "Effects");
        public static readonly string tempLevels = XPathTools.Combine(temResource, "Levels");
        public static readonly string tempUIs = XPathTools.Combine(temResource, "UIs");
        public static readonly string tempScripts = XPathTools.Combine(temResource, "Scripts");
        public static readonly string tempConfigs = XPathTools.Combine(temResource, "Configs");

        public static readonly string temMd5 = XPathTools.Combine(temResource, "_Md5");
        public static readonly string tempModelMd5s = XPathTools.Combine(temMd5, "Models");
        public static readonly string tempSpriteMd5s = XPathTools.Combine(temMd5, "Sprites");
        public static readonly string tempEffectMd5s = XPathTools.Combine(temMd5, "Effects");
        public static readonly string tempLevelMd5s = XPathTools.Combine(temMd5, "Levels");
        public static readonly string tempUIMd5s = XPathTools.Combine(temMd5, "UIs");
        public static readonly string tempScriptMd5s = XPathTools.Combine(temMd5, "Scripts");
        public static readonly string tempConfigMd5s = XPathTools.Combine(temMd5, "Configs");
    }
}

