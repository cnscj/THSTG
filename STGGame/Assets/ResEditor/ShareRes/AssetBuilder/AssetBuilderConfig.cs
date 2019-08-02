using UnityEngine;
using System.Collections;
using THEditor;
using THGame;

namespace STGGame
{
    public static class AssetBuilderConfig
    {
        public static readonly string artTempFolder = "Assets/ResEditor/Z_AutoProcess";
        public static readonly string temResource = PathUtil.Combine(artTempFolder, "ResTemp");
        public static readonly string tempModels = PathUtil.Combine(temResource, "Models");
        public static readonly string tempSprites = PathUtil.Combine(temResource, "Sprites");
        public static readonly string tempPublicFx = PathUtil.Combine(temResource, "Effects");
        public static readonly string tempLevels = PathUtil.Combine(temResource, "Levels");

        public static readonly string bundleNameLevels = "levels/{0}.ab";
        public static readonly string bundleNameEffects = "effects/{0}.ab";
        public static readonly string bundleNameSprites = "sprites/{0}.ab";
    }
}
