
using System.Collections.Generic;
using System.IO;
using ASEditor;
using STGGame;
using UnityEditor;
using UnityEngine;
using XLibrary;

namespace STGEditor
{
    public class AssetProcessorUI : ResourceProcesser
    {
        public AssetProcessorUI(string md5Folder, string exportFolder) : base(md5Folder, exportFolder)
        {
            isCheckInvainFile = false;
            isCheckFileCode = false;
            if (XFolderTools.Exists(exportFolder))
            {
                XFolderTools.DeleteDirectory(exportFolder, true);
            }
            XFolderTools.CreateDirectory(exportFolder);
        }

        protected override List<string> OnFilter()
        {
            List<string> fileList = new List<string>();
            XFolderTools.TraverseFiles(AssetProcessorConfig.srcUIs, (fullPath) =>
             {
                 //值拷贝png和bytes文件
                 string fileEx = Path.GetExtension(fullPath).ToLower();
                 if (fileEx == ".png" || fileEx == ".bytes")
                 {
                     fileList.Add(fullPath);
                 }
             });
            return fileList;
        }

        protected override void OnOnce(string assetPath)
        {
            //直接拷贝到目标目录
            string fileName = Path.GetFileName(assetPath);
            XFileTools.Copy(assetPath, string.Format("{0}/{1}", GetExportPath(),fileName));
        }
    }
}
