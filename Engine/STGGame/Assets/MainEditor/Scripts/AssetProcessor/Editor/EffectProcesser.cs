using System.IO;
using System.Linq;
using ASEditor;
using ASGame;
using STGGame;
using UnityEditor;
using UnityEngine;
using XLibrary;

namespace STGEditor
{
    public class EffectProgresser : AssetCustomProcesser
    {
        public override AssetCustomProcesserInfo OnInit()
        {
            return new AssetCustomProcesserInfo()
            {
                name = "Effect",
            };
        }

        protected override string[] OnFiles()
        {
            string[] files = Directory.GetFiles(AssetProcessorConfig.srcEffect, "*.prefab", SearchOption.AllDirectories)
                .Where(path => !Path.GetFileNameWithoutExtension(path).StartsWith("_"))
                .Where(path => !Path.GetExtension(path).Contains("meta"))
                .ToArray();
            return files;
        }

        protected override string[] OnOnce(string srcFilePath)
        {
            GameObject srcPrefab = PrefabUtility.LoadPrefabContents(srcFilePath);
            GameObject newPrefab = Object.Instantiate(srcPrefab);
            PrefabUtility.UnloadPrefabContents(srcPrefab);


            //计算特效时长
            {
                EffectLengthMono lenCom = newPrefab.GetComponent<EffectLengthMono>() ?? newPrefab.AddComponent<EffectLengthMono>();
                lenCom.Calculate();
            }



            //保存
            string saveFolderPath = GetSaveFolderPath();
            string prefabKey = XStringTools.SplitPathKey(srcFilePath);
            string savePath = Path.Combine(saveFolderPath, string.Format("{0}.prefab", prefabKey));
            PrefabUtility.SaveAsPrefabAsset(newPrefab, savePath);
            Object.DestroyImmediate(newPrefab);

            return new string[] { savePath };
        }

    }

}
