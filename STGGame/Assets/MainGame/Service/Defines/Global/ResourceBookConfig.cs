using UnityEngine;
using XLibrary;
using STGGame;
namespace STGService
{
	public static class ResourceBookConfig
    {
        public static readonly string bundleRes = PathUtil.Combine(Application.streamingAssetsPath, "ABRes", PlatformUtil.GetCurPlatformName());
        public static readonly string srcRes = PathUtil.Combine("Assets", "GameAssets");

        
    }

}
