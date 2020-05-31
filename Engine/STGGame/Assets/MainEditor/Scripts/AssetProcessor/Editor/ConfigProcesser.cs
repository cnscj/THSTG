using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASEditor;
using ASGame;
using STGGame;
using UnityEditor;
using XLibrary;

namespace STGEditor
{
    public class ConfigProcesser : AssetCustomProcesser
    {
        public override AssetCustomProcesserInfo OnInit()
        {
            return new AssetCustomProcesserInfo()
            {
                name = "Config",
            };
        }

        protected override string[] OnFiles()
        {
            string[] files = Directory.GetFiles(AssetProcessorConfig.srcConfigs, "*.xlsx", SearchOption.AllDirectories)
                .Where(path => !Path.GetFileNameWithoutExtension(path).StartsWith("~$"))
                .Where(path => !Path.GetExtension(path).Contains("meta"))
                .ToArray();
            return files;
        }

        protected override void OnOnce(string srcFilePath)
        {
            string fileNameNotEx = Path.GetFileNameWithoutExtension(srcFilePath);
            string saveFolderPath = GetSaveFolderPath();
            string savePath = Path.Combine(saveFolderPath, string.Format("{0}.csv", fileNameNotEx));

            Excel2CSV converter = new Excel2CSV(srcFilePath);
            converter.Export(savePath);
        }

        protected override AssetProcessCheckfile OnUpdate(string srcFilePath, AssetProcessCheckfile checkfile)
        {
            return null;    //不保存MD5文件
        }

    }

}
