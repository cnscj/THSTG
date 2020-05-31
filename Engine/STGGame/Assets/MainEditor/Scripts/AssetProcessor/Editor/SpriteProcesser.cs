using System.IO;
using System.Linq;
using ASEditor;
using STGGame;
using UnityEditor;
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

        protected override void OnOnce(string srcFilePath)
        {
            //拷贝所有文件
            string assetKey = XStringTools.SplitPathKey(srcFilePath);
            string saveFolderPath = GetSaveFolderPath();
            string savePath = Path.Combine(saveFolderPath, string.Format("{0}.prefab", assetKey));

            AssetDatabase.CopyAsset(srcFilePath, savePath);
        }

    }

}
