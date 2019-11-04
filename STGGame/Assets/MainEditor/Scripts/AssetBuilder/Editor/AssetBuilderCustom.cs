
using System.Collections.Generic;
using System.IO;
using ASEditor;
using STGGame;
using XLibrary;

namespace STGEditor
{
    public class AssetBuilderCustom : ResourceBuilder
    {
       
        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            XFolderTools.TraverseFiles(new string[] { AssetBuilderConfig.tempCustoms }, (fullPath) =>
            {
                string fileExName = Path.GetExtension(fullPath).ToLower();
                if (!fileExName.Contains("meta"))
                {
                    string fileRelaPath = XFileTools.GetFileRelativePath(fullPath);
                    filList.Add(fileRelaPath);
                }
            }, true);
            return filList;
        }

        protected override void OnOnce(string assetPath)
        {
            string fileNameNotEx = Path.GetFileNameWithoutExtension(assetPath);
            SetBundleName(assetPath, GetRelationPath(assetPath, fileNameNotEx));
        }

        protected override void OnShareOnce(string assetPath, int dependCount)
        {
            SetBundleName(assetPath, string.Format(AssetBuilderConfig.bundleNameCustoms, "share"));
        }


        private string GetRelationPath(string assetPath,string fileName)
        {
            string relaPath = PathUtil.GetRelativePath(AssetBuilderConfig.tempCustoms, assetPath);
            string relaRootPath = Path.GetDirectoryName(relaPath);
            return string.Format(AssetBuilderConfig.bundleNameCustoms, string.Format("{0}/{1}", relaRootPath, fileName));
        }
    }
}