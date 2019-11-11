﻿using System;
using System.Collections.Generic;
using System.IO;
using ASEditor;
using STGGame;
using UnityEditor;

namespace STGEditor
{
    public class AssetBuilderUI : ResourceBuilder
    {
        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[1] { AssetBuilderConfig.tempUIs });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                filList.Add(assetPath);
            }
            return filList;
        }
        protected string GetModuleName(string assetPath)
        {
            string moduleName = "";
            if (assetPath.Contains(AssetProcessorConfig.srcUIs))
            {
                string relaPath = assetPath.Replace(AssetProcessorConfig.srcUIs, "").Replace("\\", "/");
                string[] splitArray = relaPath.Split('/');

                moduleName = splitArray.Length > 1 ? splitArray[1] : moduleName;
            }
            return moduleName;
        }
        protected override void OnOnce(string assetPath)
        {
            string fileNameNotEx = Path.GetFileNameWithoutExtension(assetPath);
            string[] splitArray = fileNameNotEx.Split('_');
            SetBundleName(assetPath, string.Format(AssetBuilderConfig.bundleNameUIs, splitArray[0].ToLower()));
        }
        protected override void OnShareOnce(string assetPath, int dependCount)
        {
            string moduleName = GetModuleName(assetPath);
            if (moduleName == "")
            {
                SetBundleName(assetPath, string.Format(AssetBuilderConfig.bundleNameUIs, "share"));
            }
            else
            {
                SetBundleName(assetPath, string.Format(AssetBuilderConfig.bundleNameUIs, string.Format("{0}", moduleName.ToLower())));
            }
            
        }
    }
}