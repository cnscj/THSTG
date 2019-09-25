
using System.Collections.Generic;
using ASGame;
using UnityEditor;
using UnityEngine;
using XLibrary;

namespace ASEditor
{
    public class ResourceBuilderConfig : ScriptableObject
    {
        public enum BuildPlatform
        {
            Auto,
            PC,
            Android,
            IOS
        }


        public static readonly string resourcePath = "Assets/Resources";
        public static readonly string configAssetsPath = PathUtil.Combine(resourcePath, "ASResourceBuilderConfig.asset");

        private static ResourceBuilderConfig s_asset;

        //手动设置
        public BuildPlatform targetType = BuildPlatform.Auto;
        public bool isUsePlatformName = true;
        public bool isBuildShare = true;
        public bool isUseLower = true;
        public string shareBundleName = "share.ab";
        public string exportFolder = "Assets/StreamingAssets";
        public bool isClearAfterBuilded = true;
        public bool bundleIsUseFullPath = false;    //只允许全路径加载Bundle

        public List<ResourceBuilderInfos> buildInfoList = new List<ResourceBuilderInfos>();


        public static ResourceBuilderConfig GetInstance()
        {
            if (!s_asset)
            {
                s_asset = GetOrCreateAsset();
            }
            return s_asset;
        }

        public BuildTarget GetBuildType()
        {
            switch(targetType)
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

        static ResourceBuilderConfig GetOrCreateAsset()
        {
            ResourceBuilderConfig asset = null;
            if (XFileTools.Exists(configAssetsPath))
            {
                asset = AssetDatabase.LoadAssetAtPath<ResourceBuilderConfig>(configAssetsPath);
            }
            else
            {
                asset = ScriptableObject.CreateInstance<ResourceBuilderConfig>();
                if (!XFolderTools.Exists(resourcePath))
                {
                    XFolderTools.CreateDirectory(resourcePath);
                }
                AssetDatabase.CreateAsset(asset, configAssetsPath);
                AssetDatabase.Refresh();
            }
            return asset;
        }

        
    }

}
