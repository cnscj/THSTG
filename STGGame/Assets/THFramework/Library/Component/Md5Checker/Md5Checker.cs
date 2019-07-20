using System.Collections;
using System.Collections.Generic;
using System.IO;
using THGame;
using UnityEngine;
namespace THGame
{
    public class Md5Checker
    {
        public enum CheckType
        {
            File,
            Folder,
            Depends
        }
        private string m_saveMd5Folder;
        private CheckType m_checkType;

        private string m_curRecPath;
        private string m_checkMd5;

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

        public bool IsMd5Changed(CheckType type, string scrPath, string suffix = "")
        {
            string fileNotExName = Path.GetFileNameWithoutExtension(scrPath);
            string saveFolderPath = m_saveMd5Folder;
            if (saveFolderPath == "")
            {
                saveFolderPath = Path.GetDirectoryName(scrPath);
            }
            m_curRecPath = PathUtil.Combine(saveFolderPath, string.Format("{0}{1}", fileNotExName, suffix));

            m_checkMd5 = GetMd5(type, scrPath);
            string recMd5 = LoadMd5(m_curRecPath);

            if (m_checkMd5 != recMd5)
            {
                return true;
            }

            return false;

        }

        public void SaveMd5Changed()
        {
            SaveMd5(m_checkMd5, m_curRecPath);
        }

        public string GetMd5(CheckType type, string path)
        {
            string md5 = "";
            if (type == CheckType.File)
            {
                md5 = XStringTools.FileToMd5(path);
            }
            else if (type == CheckType.Folder)
            {
                //TODO:
            }
            else if (type == CheckType.Depends)
            {
                //TODO:
            }
            return md5;
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

    }

}

