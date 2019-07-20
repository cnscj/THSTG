using System.Collections;
using System.Collections.Generic;
using System.IO;
using THGame;
using UnityEditor;
using UnityEngine;
namespace THEditor
{
    public class PostProcesser
    {
        private Md5Checker md5Checker;
        private Dictionary<string, string> fileMaps;

        private string m_md5Folder;
        private string m_exportFolder;

        public PostProcesser(string md5Folder,string exportFolder)
        {
            md5Checker = new Md5Checker(md5Folder);
            fileMaps = new Dictionary<string, string>();

            m_md5Folder = md5Folder;
            m_exportFolder = exportFolder;
            if (m_exportFolder != "")
            {
                if (!XFolderTools.Exists(m_exportFolder))
                {
                    XFolderTools.CreateDirectory(m_exportFolder);
                }
            }
        }

        public virtual void Do()
        {
            DoBegin();

            var fileList = OnFilter();
            foreach (var assetPath in fileList)
            {
                DoOnce(assetPath);
            }

            DoEnd();
        }

        protected void DoBegin()
        {
            //清空
            Clear();
        }


        protected void DoEnd()
        {
            // 移除无效的二次转化文件（源文件不在了）
            XFolderTools.TraverseFiles(m_md5Folder, (fullPath) =>
            {
                string fileNameWithNotEx = Path.GetFileNameWithoutExtension(fullPath);
                string fileEx = Path.GetExtension(fullPath);
                if(fileEx.Contains("meta"))
                {
                    return;
                }
                
                if (!fileMaps.ContainsKey(fileNameWithNotEx))
                {
                    string relaPath = XFileTools.GetFileRelativePath(fullPath);
                    string relaRootPath = Path.GetDirectoryName(relaPath);
                    AssetDatabase.DeleteAsset(relaPath);

                }
            });

            XFolderTools.TraverseFiles(m_exportFolder, (fullPath) =>
            {
                string fileNameWithNotEx = Path.GetFileNameWithoutExtension(fullPath);
                string fileEx = Path.GetExtension(fullPath);
                if (fileEx.Contains("meta"))
                {
                    return;
                }

                if (!fileMaps.ContainsKey(fileNameWithNotEx))
                {
                    string relaPath = XFileTools.GetFileRelativePath(fullPath);
                    string relaRootPath = Path.GetDirectoryName(relaPath);
                    AssetDatabase.DeleteAsset(relaPath);

                    string relaFoldePath = PathUtil.Combine(relaRootPath, fileNameWithNotEx);
                    if (XFolderTools.Exists(relaFoldePath))
                    {
                        XFolderTools.DeleteDirectory(relaFoldePath);
                    }
                }
            });


        }

        protected virtual List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            return filList;
        }

        protected virtual void OnOnce(string assetPath)
        {

        }

        public void Clear()
        {
            fileMaps.Clear();
        }

        protected string GetExportPath(string fileName)
        {
            return PathUtil.Combine(m_exportFolder, fileName);
        }

        private void DoOnce(string assetPath)
        {
            if (assetPath == "")
            {
                return;
            }

            string fileNameWithNotEx = Path.GetFileNameWithoutExtension(assetPath);
            string fileName = Path.GetFileName(assetPath);

            if (fileMaps.ContainsKey(fileName))
            {
                return;
            }

            if (!md5Checker.IsMd5Changed(Md5Checker.CheckType.File, assetPath))
            {
                //MD5没变,但是目标文件被删除
                string exportFilePath = Path.Combine(m_exportFolder, fileName);
                if (XFileTools.Exists(exportFilePath))
                {
                    fileMaps.Add(fileNameWithNotEx, assetPath);
                    return;
                }
            }

            {
                OnOnce(assetPath);

                md5Checker.SaveMd5Changed();
                fileMaps.Add(fileNameWithNotEx, assetPath);
            }
        }
    }

}

