using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XLibEditor;
using XLibrary;

namespace ASEditor
{
    public class AssetBuildConfiger : ConfigObject<AssetBuildConfiger>
    {
        public enum BuildPlatform
        {
            Auto,
            PC,
            Android,
            IOS
        }

        //手动设置
        public BuildPlatform targetType = BuildPlatform.Auto;
        public bool isUsePlatformName = true;
        public bool isUseLower = true;
        public string exportFolder = "Assets/StreamingAssets";
        public bool isClearAfterBuilded = true;
        public bool bundleIsUseFullPath = false;    //只允许全路径加载Bundle

        public List<AssetCommonBuildItem> buildItemList = new List<AssetCommonBuildItem>();

        public BuildTarget GetBuildType()
        {
            switch (targetType)
            {
                case BuildPlatform.PC:
#if UNITY_STANDALONE_WIN 
                    return BuildTarget.StandaloneWindows;
#elif UNITY_STANDALONE_OSX
                    return BuildTarget.StandaloneOSX;
#endif
                case BuildPlatform.Android:
                    return BuildTarget.Android;
                case BuildPlatform.IOS:
                    return BuildTarget.iOS;
            }
            return EditorUserBuildSettings.activeBuildTarget;
        }
    }
}
