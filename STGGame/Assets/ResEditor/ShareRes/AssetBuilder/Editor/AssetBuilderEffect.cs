using System;
using System.Collections.Generic;
using System.IO;
using STGGame;
using THEditor;
using UnityEditor;
using UnityEngine;

namespace STGEditor
{
    public class AssetBuilderEffect : BundleBuilder
    {
        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[1] { AssetBuilderConfig.tempPublicFx });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                filList.Add(assetPath);
            }
            return filList;
        }

        protected override void OnOnce(string assetPath)
        {
            string fileNameNotEx = Path.GetFileNameWithoutExtension(assetPath);
            SetBundleName(assetPath, string.Format(AssetBuilderConfig.bundleNameEffects, fileNameNotEx));
        }

    }
}