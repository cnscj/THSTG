using System.Collections;
using System.Collections.Generic;
using System.IO;
using STGGame;
using THEditor;
using THGame;
using UnityEditor;
using UnityEngine;

namespace STGEditor
{
    public class PrefabBuilderProcessPublicFx : PostProcesser
    {
        public PrefabBuilderProcessPublicFx(string md5Folder, string exportFolder) : base(md5Folder, exportFolder)
        {

        }

        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[1] { PrefabBuildConfig.srcPublicFx });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                filList.Add(assetPath);
            }
            return filList;
        }
        protected override void OnPreOnce(string assetPath)
        {
            string resId = GetResourceId(assetPath);
            SetSaveCodeName(string.Format("{0}{1}", resId, "_pubfx"));
            SetExportName(string.Format("{0}.prefab", resId));
        }

        protected override void OnOnce(string assetPath)
        {
            GameObject srcPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject newPrefab = Object.Instantiate(srcPrefab);

            string savePath = GetExportPath();
            PrefabUtility.SaveAsPrefabAsset(newPrefab, savePath);
            Object.DestroyImmediate(newPrefab);
        }
    }
}
