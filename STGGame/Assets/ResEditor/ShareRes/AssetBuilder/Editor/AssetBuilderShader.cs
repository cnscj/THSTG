using System;
using System.Collections.Generic;
using System.IO;
using STGGame;
using THEditor;
using UnityEditor;
using UnityEngine;

namespace STGEditor
{
    public class AssetBuilderShader : BundleBuilder
    {
        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Shader", new string[1] { AssetBuilderConfig.srcShaders });
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
            SetBundleName(assetPath, string.Format(AssetBuilderConfig.bundleNameShaders, "share"));
        }
    }
}