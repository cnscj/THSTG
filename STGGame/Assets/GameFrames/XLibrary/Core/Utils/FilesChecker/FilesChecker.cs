
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XLibrary
{
    public class FilesChecker
    {
        public enum CodeType
        {
            Md5,

        }
        private string m_saveCodeFolder;
        private CodeType m_CheckCodeType;

        private string m_curRecPath;
        private string m_checkCode;
        private string m_saveMd5Name = "";

        public FilesChecker(string codeFolder = "", CodeType codeType = CodeType.Md5)
        {
            m_saveCodeFolder = codeFolder;
            m_CheckCodeType = codeType;
            if (m_saveCodeFolder != "")
            {
                if (!XFolderTools.Exists(m_saveCodeFolder))
                {
                    XFolderTools.CreateDirectory(m_saveCodeFolder);
                }
            }
        }

        public bool IsCodeChanged(string[] paths, string codeFileName = "")
        {
            string fileNotExName = codeFileName;
            string saveFolderPath = m_saveCodeFolder;
            if (codeFileName == "")
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
            m_checkCode = GetCode(paths);

            string recMd5 = LoadCode(m_curRecPath);

            if (m_checkCode != recMd5)
            {
                return true;
            }

            return false;

        }

        public void SetSvaeCodeName(string fileName)
        {
            m_saveMd5Name = fileName;
        }

        public string GetSvaeCodeName()
        {
            return m_saveMd5Name;
        }

        public void SaveCodeChanged()
        {
            SaveCode(m_checkCode, m_curRecPath);
        }

        public string GetCode(string[] paths)
        {
            SortedList<string, string> codeList = new SortedList<string, string>();
            foreach (var path in paths)
            {
                string code = "";
                if (Directory.Exists(path))    //是文件夹
                {
                    code = GetFolderCode(path);
                }
                else if (File.Exists(path))
                {
                    code = GetFileMd5(path);
                }
                if (!codeList.ContainsKey(code))
                    codeList.Add(code, path);
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var codePair in codeList)
            {
                string code = codePair.Key;
                stringBuilder.Append(code);
                stringBuilder.Append("|");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }

        public string LoadCode(string path)
        {
            string code = "";
            if (XFileTools.Exists(path))
            {
                code = File.ReadAllText(path);
            }
            return code;
        }

        public void SaveCode(string code, string path)
        {
            File.WriteAllText(path, code);
        }

        private string GetFileMd5(string filePath)
        {
            return XStringTools.FileToMd5(filePath);
        }

        private string GetFileHash(string filePath)
        {
            return XStringTools.FileToMd5(filePath);
        }

        private string GetFilesCode(string[] filePaths)
        {
            SortedList<string, string> codeList = new SortedList<string, string>();
            foreach (var filePath in filePaths)
            {
                string code = "";
                switch (m_CheckCodeType)
                {
                    case CodeType.Md5:
                        code = GetFileMd5(filePath);
                        break;
                }

                codeList.Add(code, filePath);
            }
            //遍历文件夹,遍历所有文件Md5
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var code in codeList)
            {
                stringBuilder.Append(code.Key);
                stringBuilder.Append("|");
            }

            return stringBuilder.ToString();
        }

        private string GetFolderCode(string folderPath)
        {
            //遍历文件夹,遍历所有文件Code
            List<string> filePaths = new List<string>();
            XFolderTools.TraverseFiles(folderPath, (fullPath) =>
            {
                string path = XFileTools.GetFolderPath(fullPath);
                filePaths.Add(path);
            }, true);

            string md5s = GetFilesCode(filePaths.ToArray());
            return XStringTools.StringToMD5(md5s);
        }

    }

}

