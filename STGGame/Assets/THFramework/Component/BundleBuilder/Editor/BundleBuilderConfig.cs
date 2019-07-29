
using System.Collections;
using System.Collections.Generic;
using THGame;
using UnityEditor;
using UnityEngine;
namespace THEditor
{
    public class BundleBuilderConfig : ScriptableObject
    {
        public enum BuildPlatform
        {
            Auto,
            PC,
            Android,
            IOS
        }


        public static readonly string resourcePath = "Assets/Resources";
        public static readonly string configAssetsPath = PathUtil.Combine(resourcePath, "THBundleBuilderConfig.asset");

        private static BundleBuilderConfig s_asset;

        //手动设置
        public BuildPlatform targetType = BuildPlatform.Auto;
        public bool isBuildShare = false;
        public string shareBundleName = "share";
        public string exportFolder = "";

        public List<BundleBuilderInfos> buildInfoList = new List<BundleBuilderInfos>();


        public static BundleBuilderConfig GetInstance()
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

        static BundleBuilderConfig GetOrCreateAsset()
        {
            BundleBuilderConfig asset = null;
            if (XFileTools.Exists(configAssetsPath))
            {
                asset = AssetDatabase.LoadAssetAtPath<BundleBuilderConfig>(configAssetsPath);
            }
            else
            {
                asset = ScriptableObject.CreateInstance<BundleBuilderConfig>();
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
