
using XLibrary;

namespace STGGame
{
    public static class AssetProcessorConfig
    {
        public static readonly string artSrcFolder = "Assets/MainEditor";

        public static readonly string srcArtResource = PathUtil.Combine(artSrcFolder, "Art");
        public static readonly string srcModels = PathUtil.Combine(srcArtResource, "Models");
        public static readonly string srcSprites = PathUtil.Combine(srcArtResource, "Sprites");
        public static readonly string srcEffect = PathUtil.Combine(srcArtResource, "Effects", "Prefab");
        public static readonly string srcPublicFx = PathUtil.Combine(srcEffect, "PublicFx");
        public static readonly string srcSkillFx = PathUtil.Combine(srcEffect, "SkillFx");
        public static readonly string srcModelFx = PathUtil.Combine(srcEffect, "ModelFx");
        public static readonly string srcUIFx = PathUtil.Combine(srcEffect, "UIFx");
        public static readonly string srcSceneFx = PathUtil.Combine(srcEffect, "SceneFx");

        public static readonly string srcLevels = PathUtil.Combine(srcArtResource, "Levels");

        public static readonly string srcProgramResource = PathUtil.Combine(artSrcFolder, "Program");
        public static readonly string srcUIs = PathUtil.Combine(srcProgramResource, "UIs");
        public static readonly string srcScripts = PathUtil.Combine(srcProgramResource, "Scripts");

        public static readonly string srcDesignResource = PathUtil.Combine(artSrcFolder, "Design");
        public static readonly string srcConfigs = PathUtil.Combine(srcDesignResource, "Configs");

        ///
        public static readonly string artTempFolder = "Assets/GameAssets";
        public static readonly string temResource = PathUtil.Combine(artTempFolder, "");
        public static readonly string tempModels = PathUtil.Combine(temResource, "Models");
        public static readonly string tempSprites = PathUtil.Combine(temResource, "Sprites");
        public static readonly string tempEffect = PathUtil.Combine(temResource, "Effects");
        public static readonly string tempLevels = PathUtil.Combine(temResource, "Levels");
        public static readonly string tempUIs = PathUtil.Combine(temResource, "UIs");
        public static readonly string tempScripts = PathUtil.Combine(temResource, "Scripts");
        public static readonly string tempConfigs = PathUtil.Combine(temResource, "Configs");

        public static readonly string temMd5 = PathUtil.Combine(temResource, "_Md5");
        public static readonly string tempModelMd5s = PathUtil.Combine(temMd5, "Models");
        public static readonly string tempSpriteMd5s = PathUtil.Combine(temMd5, "Sprites");
        public static readonly string tempEffectMd5s = PathUtil.Combine(temMd5, "Effects");
        public static readonly string tempLevelMd5s = PathUtil.Combine(temMd5, "Levels");
        public static readonly string tempUIMd5s = PathUtil.Combine(temMd5, "UIs");
        public static readonly string tempScriptMd5s = PathUtil.Combine(temMd5, "Scripts");
        public static readonly string tempConfigMd5s = PathUtil.Combine(temMd5, "Configs");
    }
}

