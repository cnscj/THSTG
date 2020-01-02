
using System.Collections.Generic;
using ASEditor;
using ASGame;
using STGGame;
using UnityEditor;
using UnityEngine;

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
