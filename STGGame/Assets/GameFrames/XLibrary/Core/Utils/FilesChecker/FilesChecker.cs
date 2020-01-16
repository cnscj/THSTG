
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace XLibrary
{
    public class FilesChecker
    {
        public enum CodeType
        {
            Md5,
            Sha1,

        }
        private string m_saveCodeFolder;
        private CodeType m_CheckCodeType;

        private string m_curRecPath;
        private string m_checkCode;
        private string m_saveCodeName = "";

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

            m_curRecPath = XPathTools.Combine(saveFolderPath, string.Format("{0}", (m_saveCodeName == "" ? fileNotExName : m_saveCodeName)));
            m_checkCode = GetCode(paths);

            string recCode = LoadCode(m_curRecPath);

            if (m_checkCode != recCode)
            {
                return true;
            }

            return false;

        }

        public void SetSvaeCodeName(string fileName)
        {
            m_saveCodeName = fileName;
        }

        public string GetSvaeCodeName()
        {
            return m_saveCodeName;
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
                    code = GetFileCode(path);
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

        private string GetFileCode(string filePath)
        {
            string code = "";
            switch (m_CheckCodeType)
            {
                case CodeType.Md5:
                    code = GetFileMd5(filePath);
                    break;
                case CodeType.Sha1:
                    code = GetFileSha1(filePath);
                    break;
            }
            return code;
        }

        private string GetFilesCode(string[] filePaths)
        {
            SortedList<string, string> codeList = new SortedList<string, string>();
            foreach (var filePath in filePaths)
            {
                string code = GetFileCode(filePath);
                codeList.Add(code, filePath);
            }
            //遍历文件夹,遍历所有文件Code
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

            string codes = GetFilesCode(filePaths.ToArray());
            return codes;
        }


        //
        private string GetFileMd5(string filePath)
        {
            return XStringTools.FileToMd5(filePath);
        }

        private string GetFileSha1(string filePath)
        {
            var hash = SHA1.Create();
            var stream = new FileStream(filePath, FileMode.Open);
            byte[] hashByte = hash.ComputeHash(stream);
            stream.Close();
            return BitConverter.ToString(hashByte).Replace("-", "");
        }
    }

}

