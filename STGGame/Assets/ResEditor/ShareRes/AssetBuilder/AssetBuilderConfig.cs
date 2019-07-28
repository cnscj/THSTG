using UnityEngine;
using System.Collections;
using THEditor;
using THGame;

namespace STGEditor
{
    public static class AssetBuilderConfig
    {
        public static readonly string artTempFolder = "Assets/ResEditor/Z_AutoProcess";
        public static readonly string temResource = PathUtil.Combine(artTempFolder, "ResTemp");
        public static readonly string tempModels = PathUtil.Combine(temResource, "Models");
        public static readonly string tempSprites = PathUtil.Combine(temResource, "Sprites");
        public static readonly string tempPublicFx = PathUtil.Combine(temResource, "Effects");
        public static readonly string tempLevels = PathUtil.Combine(temResource, "Levels");

        public static readonly string bundleFolder = "Assets/ResEditor/Z_AutoProcess";
        public static readonly string bundleResource = PathUtil.Combine(bundleFolder, "AssetBundle");
        public static readonly string bundleLevels = PathUtil.Combine(bundleResource, "Levels");
        public static readonly string bundleEffects = PathUtil.Combine(bundleResource, "Effects");
    }
}
