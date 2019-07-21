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
        private Dictionary<string, string> checkMaps;

        private string m_md5Folder;
        private string m_exportFolder;

        private Md5Checker.CheckType m_md5CheckType = Md5Checker.CheckType.File;
        private string m_exportFilePath = "";

        public PostProcesser(string md5Folder,string exportFolder)
        {
            md5Checker = new Md5Checker(md5Folder);
            checkMaps = new Dictionary<string, string>();

            m_md5Folder = md5Folder;
            m_exportFolder = exportFolder;
            
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

            if (m_md5Folder != "")
            {
                if (!XFolderTools.Exists(m_md5Folder))
                {
                    XFolderTools.CreateDirectory(m_md5Folder);
                }
            }

            if (m_exportFolder != "")
            {
                if (!XFolderTools.Exists(m_exportFolder))
                {
                    XFolderTools.CreateDirectory(m_exportFolder);
                }
            }
        }


        protected void DoEnd()
        {
            // 移除无效的二次转化文件（源文件不在了）
            XFolderTools.TraverseFiles(m_md5Folder, (fullPath) =>
            {
                string fileNameWithNotEx = Path.GetFileNameWithoutExtension(fullPath);
                string fileEx = Path.GetExtension(fullPath);
                string resId = GetResourceId(fileNameWithNotEx);
                if (fileEx.Contains("meta"))
                {
                    return;
                }
                
                if (!checkMaps.ContainsKey(resId))
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
                string resId = GetResourceId(fileNameWithNotEx);
                if (fileEx.Contains("meta"))
                {
                    return;
                }

                if (!checkMaps.ContainsKey(resId))
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

            AssetDatabase.Refresh();
        }

        protected virtual List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            return filList;
        }
        protected virtual void OnPreOnce(string assetPath)
        {

        }
        protected virtual void OnOnce(string assetPath)
        {

        }

        public void Clear()
        {
            checkMaps.Clear();
            m_md5CheckType = Md5Checker.CheckType.File;
            m_exportFilePath = "";

    }
    protected void SetSaveMd5Name(string md5Name)
        {
            md5Checker.SetSvaeMd5Name(md5Name);
        }
        protected void SetExportName(string exportName)
        {
            m_exportFilePath = PathUtil.Combine(m_exportFolder, exportName);
        }

        protected void SetMd5CheckType(Md5Checker.CheckType type)
        {
            m_md5CheckType = type;
        }

        protected string GetExportPath(string fileName = "")
        {
            return m_exportFilePath == "" ? Path.Combine(m_exportFolder, fileName) : m_exportFilePath;
        }
        protected string GetResourceId(string path)
        {
            return XStringTools.SplitPathId(Path.GetFileNameWithoutExtension(path));
        }
        private void DoOnce(string assetPath)
        {
            if (assetPath == "")
            {
                return;
            }

            OnPreOnce(assetPath);

            string fileNameWithNotEx = Path.GetFileNameWithoutExtension(assetPath);
            string fileName = Path.GetFileName(assetPath);
            string checkName = GetResourceId(fileName);
            string saveFilePath = GetExportPath(fileName);

            if (checkMaps.ContainsKey(checkName))
            {
                return;
            }

            if (!md5Checker.IsMd5Changed(m_md5CheckType, assetPath))
            {
                //MD5没变,但是目标文件被删除
                if (XFileTools.Exists(saveFilePath))
                {
                    checkMaps.Add(checkName, assetPath);
                    return;
                }
            }

            {
                OnOnce(assetPath);

                md5Checker.SaveMd5Changed();
                checkMaps.Add(checkName, assetPath);
            }
        }
    }

}

