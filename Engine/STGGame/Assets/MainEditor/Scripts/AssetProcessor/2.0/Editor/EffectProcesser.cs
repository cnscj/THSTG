using System.IO;
using System.Linq;
using ASEditor;

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
            string[] files = Directory.GetFiles(AssetFileBook.srcEffect, "*.prefab", SearchOption.AllDirectories).ToArray();
            return files;
        }

        protected override void OnOnce(string srcFilePath)
        {

        }

    }

}
