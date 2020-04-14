
using System.Collections.Generic;
using System.IO;
using ASEditor;
using STGGame;
using XLibrary;

namespace STGEditor
{
    public class AssetBuilderCfg : ResourceBuilder
    {
        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            XFolderTools.TraverseFiles(new string[] { AssetBuilderConfig.tempConfigs }, (fullPath) =>
            {
                string fileExName = Path.GetExtension(fullPath).ToLower();
                if (fileExName.Contains("csv") || fileExName.Contains("json"))
                {
                    string fileRelaPath = XPathTools.GetRelativePath(fullPath);
                    filList.Add(fileRelaPath);
                }
            });
            return filList;
        }

        protected override void OnOnce(string assetPath)
        {
            string fileNameNotEx = Path.GetFileNameWithoutExtension(assetPath);
            SetBundleName(assetPath, string.Format(AssetBuilderConfig.bundleNameConfigs, fileNameNotEx));
        }

        protected override void OnShareOnce(string assetPath, int dependCount)
        {
            SetBundleName(assetPath, string.Format(AssetBuilderConfig.bundleNameConfigs, "share"));
        }
    }
}