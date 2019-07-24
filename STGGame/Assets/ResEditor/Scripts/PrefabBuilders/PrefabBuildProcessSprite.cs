using System.Collections;
using System.Collections.Generic;
using System.IO;
using THEditor;
using UnityEditor;
using UnityEngine;

namespace STGEditor
{
    public class PrefabBuildProcessSprite : PostProcesser
    {
        public PrefabBuildProcessSprite(string md5Folder, string exportFolder) : base(md5Folder, exportFolder)
        {
        }

        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[1] { PrefabBuildConfig.srcSprites });
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
