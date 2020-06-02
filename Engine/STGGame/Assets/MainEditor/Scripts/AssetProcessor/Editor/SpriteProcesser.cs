using System.IO;
using System.Linq;
using ASEditor;
using STGGame;
using UnityEditor;
using UnityEngine;
using XLibrary;

namespace STGEditor
{
    public class SpriteProgresser : AssetCustomProcesser
    {
        public override AssetCustomProcesserInfo OnInit()
        {
            return new AssetCustomProcesserInfo()
            {
                name = "Sprite",
            };
        }

        protected override string[] OnFiles()
        {
            string[] files = Directory.GetFiles(AssetProcessorConfig.srcSprites, "*.prefab", SearchOption.AllDirectories)
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

            //装上box collider
            {

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
