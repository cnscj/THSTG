using System.IO;
using System.Linq;
using ASEditor;

namespace STGEditor
{
    public class ModelEffectProgresser : AssetCustomProcesser
    {
        public override AssetCustomProcesserInfo OnInit()
        {
            return new AssetCustomProcesserInfo()
            {
                name = "ModelEffect",
            };
        }

        protected override string[] OnFiles()
        {
            string[] files = Directory.GetFiles(AssetFileBook.srcModelEffect, "*.prefab", SearchOption.AllDirectories).ToArray();
            return files;
        }

        protected override void OnOnce(string srcFilePath)
        {

        }

    }

}
