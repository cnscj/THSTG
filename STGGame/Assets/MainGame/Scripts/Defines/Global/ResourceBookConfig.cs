using UnityEngine;
using System.Collections;
using THGame;
using XLibrary;
using System.Collections.Generic;

namespace STGGame
{
	public static class ResourceBookConfig
    {
        public static readonly string bundleRes = PathUtil.Combine(Application.streamingAssetsPath, "ABRes", PlatformUtil.GetCurPlatformName());
        public static readonly string srcRes = PathUtil.Combine("Assets", "GameAssets");

        
    }

}
