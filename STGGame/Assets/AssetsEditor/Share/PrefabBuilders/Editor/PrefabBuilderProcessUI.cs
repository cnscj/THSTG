
using System.Collections.Generic;
using System.IO;
using ASEditor;
using STGGame;
using UnityEditor;
using UnityEngine;

namespace STGEditor
{
    public class PrefabBuilderProcessUI : ResourceProcesser
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
            if (assetPath.Contains(PrefabBuildConfig.srcUIs))
            {
                string relaPath = assetPath.Replace(PrefabBuildConfig.srcUIs, "").Replace("\\", "/");
                string[] splitArray = relaPath.Split('/');

                moduleName = splitArray.Length > 1 ? splitArray[1] : moduleName;
            }
            return moduleName;
        }

        protected override void OnPreOnce(string assetPath)
        {
            //md5格式:模块名_自身名
            //取得模块名
            string moduleName = GetModuleName(assetPath);
            string fileNameNotEx = Path.GetFileNameWithoutExtension(assetPath);
            if (moduleName == "")
            {
                SetSaveCodeName(string.Format("{0}", fileNameNotEx));
                SetExportName(string.Format("{0}.prefab", fileNameNotEx));
            }
            else
            {
                SetSaveCodeName(string.Format("{0}_{1}", moduleName, fileNameNotEx));
                SetExportName(string.Format("{0}_{1}.prefab", moduleName, fileNameNotEx));
            }

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
