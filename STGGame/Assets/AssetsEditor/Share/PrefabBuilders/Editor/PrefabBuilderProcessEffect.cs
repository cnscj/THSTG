
using System.Collections.Generic;
using ASEditor;
using STGGame;
using UnityEditor;
using UnityEngine;

namespace STGEditor
{
    public class PrefabBuilderProcessEffect : ResourceProcesser
    {
        public PrefabBuilderProcessEffect(string md5Folder, string exportFolder) : base(md5Folder, exportFolder)
        {

        }

        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[1] { PrefabBuildConfig.srcEffect });
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
            SetSaveCodeName(string.Format("{0}", resId));
            SetExportName(string.Format("{0}.prefab", resId));
        }

        protected override void OnOnce(string assetPath)
        {

            GameObject prefab = null;
            Do4FxBegin(assetPath, out prefab);
            if (prefab != null)
            {
                if (assetPath.Contains(PrefabBuildConfig.srcPublicFx))
                {
                    Do4PublicFx(prefab);
                }
            }
            Do4FxEnd(prefab);
        }

        ///
        private void Do4FxBegin(string assetPath,out GameObject gameObject)
        {
            GameObject srcPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject newPrefab = Object.Instantiate(srcPrefab);

            gameObject = newPrefab;
        }

        private void Do4FxEnd(GameObject GO)
        {
            if (GO != null)
            {
                string savePath = GetExportPath();
                PrefabUtility.SaveAsPrefabAsset(GO, savePath);
                Object.DestroyImmediate(GO);
            }

        }
        //
        private void Do4PublicFx(GameObject GO)
        {
            
        }
    }
}
