using System.IO;
using System.Linq;
using ASEditor;
using STGGame;
using UnityEditor;
using XLibrary;

namespace STGEditor
{
    public class FGUIProcesser : AssetCustomProcesser
    {
        public override AssetCustomProcesserInfo OnInit()
        {
            return new AssetCustomProcesserInfo()
            {
                name = "FGUI",
            };
        }

        protected override string[] OnFiles()
        {
            string[] files = Directory.GetFiles(AssetProcessorConfig.srcUIs, "*.*", SearchOption.AllDirectories)
                .Where(path => !Path.GetExtension(path).Contains("meta"))
                .ToArray();
            return files;
        }

        protected override string[] OnOnce(string srcFilePath)
        {
            //拷贝所有文件
            string fileName = Path.GetFileName(srcFilePath);
            string saveFolderPath = GetSaveFolderPath();
            string savePath = Path.Combine(saveFolderPath, fileName);

            XFileTools.Copy(srcFilePath, savePath, true);

            return new string[] { savePath };
            //AssetDatabase.CopyAsset(srcFilePath, savePath);   //这个函数没法拷贝纹理
        }

        protected override AssetProcessCheckfile OnUpdate(string srcFilePath, AssetProcessCheckfile checkfile)
        {
            return null;    //不保存MD5文件
        }
    }

}
