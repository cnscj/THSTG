
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace THGame
{
    public class Md5Checker
    {
        private string m_saveMd5Folder;

        private string m_curRecPath;
        private string m_checkMd5;
        private string m_saveMd5Name = "";

        public Md5Checker(string md5Folder = "")
        {
            m_saveMd5Folder = md5Folder;
            if(m_saveMd5Folder != "")
            {
                if (!XFolderTools.Exists(m_saveMd5Folder))
                {
                    XFolderTools.CreateDirectory(m_saveMd5Folder);
                }
            }
        }
        
        public bool IsMd5Changed(string []paths, string md5FileName = "")
        {
            string fileNotExName = md5FileName;
            string saveFolderPath = m_saveMd5Folder;
            if (md5FileName == "")
            {
                if (paths.Length >= 1)
                {
                    fileNotExName = Path.GetFileNameWithoutExtension(paths[0]);
                    if (saveFolderPath == "")
                    {
                        saveFolderPath = Path.GetDirectoryName(paths[0]);
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {

                if (saveFolderPath == "")
                {
                    if (paths.Length >= 1)
                    {
                        saveFolderPath = Path.GetDirectoryName(paths[0]);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            m_curRecPath = PathUtil.Combine(saveFolderPath, string.Format("{0}", (m_saveMd5Name == "" ? fileNotExName : m_saveMd5Name)));
            m_checkMd5 = GetMd5(paths);

            string recMd5 = LoadMd5(m_curRecPath);

            if (m_checkMd5 != recMd5)
            {
                return true;
            }

            return false;

        }

        public void SetSvaeMd5Name(string fileName)
        {
            m_saveMd5Name = fileName;
        }

        public string GetSvaeMd5Name()
        {
            return m_saveMd5Name;
        }

        public void SaveMd5Changed()
        {
            SaveMd5(m_checkMd5, m_curRecPath);
        }

        public string GetMd5(string []paths)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var path in paths)
            {
                if (Directory.Exists(path))    //是文件夹
                {
                    stringBuilder.Append(GetFolderMd5(path));
                }
                else if (File.Exists(path))
                {
                    stringBuilder.Append(GetFileMd5(path));
                }
                stringBuilder.Append("|");
            }
            stringBuilder.Remove(stringBuilder.Length-1,1);
            return stringBuilder.ToString();
        }

        public string LoadMd5(string path)
        {
            string md5 = "";
            if (XFileTools.Exists(path))
            {
                md5 = File.ReadAllText(path);
            }
            return md5;
        }

        public void SaveMd5(string md5, string path)
        {
            File.WriteAllText(path, md5);
        }

        private string GetFileMd5(string filePath)
        {
            return XStringTools.FileToMd5(filePath);
        }

        private string GetFilesMd5(string []filePaths)
        {
            SortedList<string, string> md5List = new SortedList<string, string>();
            foreach(var filePath in filePaths)
            {
                string md5 = GetFileMd5(filePath);
                md5List.Add(md5, filePath);
            }
            //遍历文件夹,遍历所有文件Md5
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var md5 in md5List)
            {
                stringBuilder.Append(md5.Key);
                stringBuilder.Append("|");
            }
            
            return stringBuilder.ToString();
        }

        private string GetFolderMd5(string folderPath)
        {
            //遍历文件夹,遍历所有文件Md5
            List<string> filePaths = new List<string>();
            XFolderTools.TraverseFiles(folderPath, (fullPath) => {
                string path = XFileTools.GetFolderPath(fullPath);
                filePaths.Add(path);
            }, true);

            string md5s = GetFilesMd5(filePaths.ToArray());  
            return XStringTools.StringToMD5(md5s);
        }

    }

}

