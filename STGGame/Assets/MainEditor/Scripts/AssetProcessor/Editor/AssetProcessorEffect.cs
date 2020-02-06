
using System.Collections.Generic;
using System.IO;
using ASEditor;
using ASGame;
using STGGame;
using UnityEditor;
using UnityEngine;
using XLibrary;

namespace STGEditor
{
    public class AssetProcessorEffect : ResourceProcesser
    {
        public AssetProcessorEffect(string md5Folder, string exportFolder) : base(md5Folder, exportFolder)
        {

        }

        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[1] { AssetProcessorConfig.srcEffect });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string assetKey = XStringTools.SplitPathKey(assetPath);
                if (!string.IsNullOrEmpty(assetKey))
                {
                    filList.Add(assetPath);
                }
            }
            return filList;
        }
        protected override void OnPreOnce(string assetPath)
        {
            string resId = GetResourceId(assetPath);
            string resKey = GetResourceKey(assetPath);
            string outName = resId ?? resKey ?? Path.GetFileNameWithoutExtension(assetPath);

            SetSaveCodeName(string.Format("{0}", outName));
            SetExportName(string.Format("{0}.prefab", outName));
        }

        protected override void OnOnce(string assetPath)
        {

            GameObject prefab = null;
            Do4FxBegin(assetPath, out prefab);
            if (prefab != null)
            {
                //公共部分处理
                //计算特效时长
                {
                    EffectLengthMono lenCom = prefab.GetComponent<EffectLengthMono>() ?? prefab.AddComponent<EffectLengthMono>();
                    lenCom.Calculate();
                }

                //各自的
                if (assetPath.Contains(AssetProcessorConfig.srcPublicFx))
                {
                    Do4PublicFx(prefab);
                }
                else if(assetPath.Contains(AssetProcessorConfig.srcModelFx))
                {
                    Do4ModelFx(prefab);
                }
                else
                {
                    Do4Normal(prefab);
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
                Object.DestroyImmediate(GO);
            }

        }
        //
        private void Do4Normal(GameObject GO)
        {
            string savePath = GetExportPath();
            PrefabUtility.SaveAsPrefabAsset(GO, savePath);
        }
        private void Do4PublicFx(GameObject GO)
        {
            Do4Normal(GO);
        }
        private void Do4ModelFx(GameObject srcGO)
        {
            GameObject newGO = new GameObject();
            NodeEffectEditorTool.PackageNodeEffect(srcGO, newGO);

            string savePath = GetExportPath();
            PrefabUtility.SaveAsPrefabAsset(newGO, savePath);
            Object.DestroyImmediate(newGO);

        }
    }
}
