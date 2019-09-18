using System.Collections.Generic;
using System.IO;
using ASEditor;
using ASGame;
using STGGame;
using XLibrary;

namespace STGEditor
{
    public class PrefabBuilderProcessCfg : ResourceProcesser
    {
        public PrefabBuilderProcessCfg(string md5Folder, string exportFolder) : base(md5Folder, exportFolder)
        {
            isCheckInvainFile = false;
        }

        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();

            XFolderTools.TraverseFiles(PrefabBuildConfig.srcConfigs, (fullPath) =>
             {
                 string fileNameWithNotEx = Path.GetFileNameWithoutExtension(fullPath);

                 if (fileNameWithNotEx.StartsWith("~$", System.StringComparison.Ordinal))
                 {
                     return;
                 }

                 string fileExName = Path.GetExtension(fullPath);
                 if (fileExName.Contains("xlsx"))
                 {
                     string fileRelaPath = XFileTools.GetFileRelativePath(fullPath);
                     filList.Add(fileRelaPath);
                 }
             });
            return filList;
        }

        protected override void OnPreOnce(string assetPath)
        {
            string fileNameNotEx = Path.GetFileNameWithoutExtension(assetPath);

            SetSaveCodeName(string.Format("{0}", fileNameNotEx));
            SetExportName(string.Format("{0}.csv", fileNameNotEx));
        }

        protected override void OnOnce(string assetPath)
        {
            Excel2CSV converter = new Excel2CSV(assetPath);

            string savePath = GetExportPath();
            converter.Export(savePath);
        }
    }
}
