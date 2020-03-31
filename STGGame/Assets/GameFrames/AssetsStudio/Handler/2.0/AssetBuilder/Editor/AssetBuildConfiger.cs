using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XLibEditor;
using XLibrary;

namespace ASEditor
{
    public class AssetBuildConfiger : ConfigObject<AssetBuildConfiger>
    {
        public static readonly string DEFAULT_EXPORT_PATH = "Assets/AssetBundles";
        public static readonly string DEFAULT_BUILD_SUFFFIX = ".unity3d";

        public enum BuildPlatform
        {
            Auto,
            PC,
            Android,
            IOS
        }

        //手动设置
        public BuildPlatform targetType = BuildPlatform.Auto;
        public string exportFolder = DEFAULT_EXPORT_PATH;
        public string bundleSuffix = DEFAULT_BUILD_SUFFFIX;
        public bool isClearAfterBuilded = true;

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

        public string GetExportFolderPath()
        {
            string newExportFolder = string.IsNullOrEmpty(exportFolder) ? DEFAULT_EXPORT_PATH : exportFolder;
            string buildPlatformStr = Enum.GetName(typeof(ResourceBuilderConfig.BuildPlatform), ResourceBuilderConfig.GetInstance().targetType);
            string retExportPath = Path.Combine(newExportFolder, buildPlatformStr);
            return retExportPath;
        }

        public string GetBuildFolderPath(string builderName)
        {
            string newExportFolder = string.IsNullOrEmpty(exportFolder) ? DEFAULT_EXPORT_PATH : exportFolder;
            string newExportFolderPath = Path.Combine(newExportFolder, builderName);
            return newExportFolderPath.ToLower();
        }

        public string GetBuildBundleName(string builderName, string assetPath, bool isCommonKey = false)
        {
            string newBundleSuffix = string.IsNullOrEmpty(bundleSuffix) ? DEFAULT_BUILD_SUFFFIX : bundleSuffix;
            string buildFolderPath = GetBuildFolderPath(builderName);
            string assetNameNotEx = Path.GetFileNameWithoutExtension(assetPath);
            string assetFinalName = assetNameNotEx;
            if (isCommonKey)
            {
                assetFinalName = XStringTools.SplitPathKey(assetPath);
            }
            string bundleFileName = string.Format("{0}{1}", assetFinalName, newBundleSuffix);
            string bundleName = Path.Combine(buildFolderPath, bundleFileName);
            return bundleName.ToLower();
        }
    }
}
