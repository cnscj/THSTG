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
    public class PrefabBuilderProcessUI : PostProcesser
    {
        public PrefabBuilderProcessUI(string md5Folder, string exportFolder) : base(md5Folder, exportFolder)
        {

        }

        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[1] { PrefabBuildConfig.srcUIs });
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
            string relaPath = assetPath.Replace(PrefabBuildConfig.srcUIs, "");
            string[] splitArray = moduleName.Split('/');
            moduleName = splitArray.Length > 0 ? splitArray[0] : moduleName;
            return moduleName;
        }

        protected override void OnPreOnce(string assetPath)
        {
            //md5格式:模块名_自身名
            //取得模块名
            string moduleName = GetModuleName(assetPath);
            string fileNameNotEx = Path.GetFileNameWithoutExtension(assetPath);

            SetSaveCodeName(string.Format("{0}_{1}", moduleName, fileNameNotEx));
            SetExportName(string.Format("{0}_{1}.prefab", moduleName, fileNameNotEx));
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
