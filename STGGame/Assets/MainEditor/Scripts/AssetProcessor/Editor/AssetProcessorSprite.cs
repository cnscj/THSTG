using System.Collections.Generic;
using System.IO;
using ASEditor;
using STGGame;
using UnityEditor;
using UnityEngine;

namespace STGEditor
{
    public class AssetProcessorSprite : ResourceProcesser
    {
        public AssetProcessorSprite(string md5Folder, string exportFolder) : base(md5Folder, exportFolder)
        {
        }

        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[1] { AssetProcessorConfig.srcSprites });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                filList.Add(assetPath);
            }
            return filList;
        }

        protected override void OnOnce(string assetPath)
        {
            GameObject srcPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject newPrefab = Object.Instantiate(srcPrefab);


            string savePath = GetExportPath(Path.GetFileName(assetPath));
            PrefabUtility.SaveAsPrefabAsset(newPrefab, savePath);
            Object.DestroyImmediate(newPrefab);
        }


    }
}
