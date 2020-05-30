using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary;

namespace STGEditor
{
    public static class AssetFileBook
    {
        public static readonly string artSrcFolder = "Assets/MainEditor";
        public static readonly string srcArtResource = XPathTools.Combine(artSrcFolder, "Art");

        public static readonly string srcModels = XPathTools.Combine(srcArtResource, "Models");
        public static readonly string srcSprites = XPathTools.Combine(srcArtResource, "Sprites");
        public static readonly string srcEffect = XPathTools.Combine(srcArtResource, "Effects", "Prefab");
        public static readonly string srcModelEffect = XPathTools.Combine(srcArtResource, "Effects", "Prefab");
        //
        public static readonly string artTempFolder = "Assets/GameAssets";
        public static readonly string temResource = XPathTools.Combine(artTempFolder, "");
        public static readonly string tempModels = XPathTools.Combine(temResource, "Models");
        public static readonly string tempSprites = XPathTools.Combine(temResource, "Sprites");
        public static readonly string tempEffect = XPathTools.Combine(temResource, "Effects");
        public static readonly string tempModelEffect = XPathTools.Combine(temResource, "ModelEffects");
    }

}

