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
        public static readonly string DEFAULT_BUILD_SHARE_BUNDLE_NAME = "share/{assetFlatPath}{assetNameNotEx}.unity3d";
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
        public string shareBundleName = DEFAULT_BUILD_SHARE_BUNDLE_NAME;
        public bool isCombinePlatformName = true;

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
            string retExportPath = newExportFolder;
            if (isCombinePlatformName)
            {
                string buildPlatformStr = Enum.GetName(typeof(BuildPlatform), targetType);
                retExportPath = Path.Combine(newExportFolder, buildPlatformStr);
            }
            return XPathTools.NormalizePath(retExportPath);
        }

        public string GetBuildFolderPath(string builderName)
        {
            string newExportFolderPath = Path.Combine("", builderName);
            return XPathTools.NormalizePath(newExportFolderPath).ToLower();
        }

        public string GetBuildBundleCommonName(string builderName, string assetPath, bool isCommonKey = false)
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

        public string GetBuildBundleShareName(string assetPath)
        {
            if (!string.IsNullOrEmpty(shareBundleName))
            {
                return GetFormatBundleName(shareBundleName, assetPath);
            }
            else
            {
                assetPath = XPathTools.GetRelativePath(assetPath);
                string newBundleSuffix = string.IsNullOrEmpty(bundleSuffix) ? DEFAULT_BUILD_SUFFFIX : bundleSuffix;
                string assetNameLow = Path.GetFileNameWithoutExtension(assetPath).ToLower();
                string assetFlatPath = GetFlatPath(assetPath);
                string bundleName = string.Format("share/{0}_{1}{2}", assetFlatPath, assetNameLow, newBundleSuffix);
                return bundleName.ToLower();
            }
        }

        public string GetFlatPath(string assetPath)
        {
            assetPath = XPathTools.GetRelativePath(assetPath);
            string assetParentPath = Path.GetDirectoryName(assetPath).ToLower();
            string relaPath = assetParentPath.Replace("assets", "");
            string flatPath = relaPath.Replace("/", "_");
            if (!string.IsNullOrEmpty(flatPath))
            {
                string newFlatPath = flatPath.Remove(0, 1);
                newFlatPath = string.Format("{0}_", newFlatPath);
                return newFlatPath;
            }
            else
            {
                return "";
            }
        }

        public string GetFormatBundleName(string formatPath, string assetPath)
        {
            if (!string.IsNullOrEmpty(formatPath))
            {
                string assetRootPath = Path.GetDirectoryName(assetPath);
                string assetEx = Path.GetExtension(assetPath);
                string assetName = Path.GetFileName(assetPath);
                string assetNameNotEx = Path.GetFileNameWithoutExtension(assetPath);
                string assetFlatPath = GetFlatPath(assetPath);
                string assetKey = XStringTools.SplitPathKey(assetPath);

                string nameFormat = formatPath;
                nameFormat = nameFormat.Replace("{assetRootPath}", assetRootPath);
                nameFormat = nameFormat.Replace("{assetEx}", assetEx);
                nameFormat = nameFormat.Replace("{assetNameNotEx}", assetNameNotEx);
                nameFormat = nameFormat.Replace("{assetName}", assetName);
                nameFormat = nameFormat.Replace("{assetKey}", assetKey);
                nameFormat = nameFormat.Replace("{assetFlatPath}", assetFlatPath);

                return nameFormat;
            }
            return null;
        }
    }
}
