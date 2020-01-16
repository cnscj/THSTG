using UnityEngine;
using XLibrary;
using STGGame;
namespace STGU3D
{
	public static class ResourceBookConfig
    {
        public static readonly string bundleRes = XPathTools.Combine(Application.streamingAssetsPath, "ABRes", PlatformUtil.GetCurPlatformName());
        public static readonly string srcRes = XPathTools.Combine("Assets", "GameAssets");

        
    }

}
