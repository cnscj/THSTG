using System.Collections.Generic;
using System.IO;
using ASEditor;
using STGGame;
using UnityEditor;

namespace STGEditor
{
    public class AssetBuilderShader : ResourceBuilder
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
            SetBundleName(assetPath, string.Format(AssetBuilderConfig.bundleNameShaders, "share"));
        }
    }
}