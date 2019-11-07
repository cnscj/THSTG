using UnityEngine;
using XLibrary;
using STGGame;
namespace STGU3D
{
	public static class ResourceBookConfig
    {
        public static readonly string bundleRes = PathUtil.Combine(Application.streamingAssetsPath, "ABRes", PlatformUtil.GetCurPlatformName());
        public static readonly string srcRes = PathUtil.Combine("Assets", "GameAssets");

        
    }

}
