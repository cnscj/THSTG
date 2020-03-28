using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using XLibrary;
using XLibrary.Package;
/*
 * 二次处理尽量少生成文件,如果已经存在,务必在原基础上改
 * 尽量确保GUID不变,引用不变
 */
namespace ASEditor
{
    public abstract class AssetBaseProcesser
    {
        protected class FileInfo
        {
            public string path;
            public string md5;
        }
        protected string destFolder;
        protected Dictionary<string, FileInfo> _checkMap = new Dictionary<string, FileInfo>();

        public virtual void Do()
        {
            OnStart();

            string[] checkFiles = OnFiles();
            if (checkFiles == null || checkFiles.Length < 0)
                return;

            List<FileInfo> procressList = new List<FileInfo>();
            foreach (var file in checkFiles)
            {
                string realPath = XFileTools.GetFileRelativePath(file);

                if (_checkMap.ContainsKey(realPath))
                    continue;

                string md5 = OnMd5(realPath);

                //判断Md5

                FileInfo fileInfo = new FileInfo();
                fileInfo.path = realPath;
                fileInfo.md5 = md5;

                _checkMap.Add(realPath, fileInfo);

            }

            foreach (var doFileInfo in procressList)
            {
                var realPath = doFileInfo.path;
                OnOnce(realPath);

                //保存MD5


            }

            OnEnd();
        }
        ////////////
        protected string GetFilesMd5(string[] filesPath)
        {
            //遍历文件夹,遍历所有文件Md5
            SortedDictionary<string, string> md5Map = new SortedDictionary<string, string>();
            foreach (var filePath in filesPath)
            {
                string md5 = XStringTools.FileToMd5(filePath);
                if (!md5Map.ContainsKey(md5))
                {
                    md5Map.Add(md5, filePath);
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var pair in md5Map)
            {
                stringBuilder.Append(pair.Key);
                stringBuilder.Append("|");
            }

            string finalMd5 = XStringTools.StringToMD5(stringBuilder.ToString());
            return finalMd5;
        }

        protected string GetFileDepsMd5(string filePath, string []exArray = null)
        {
            string []dependences = AssetDatabase.GetDependencies(filePath);
            string []checkFile = dependences;
            if (exArray != null)
            {
                List<string> needFiles = new List<string>();
                HashSet<string> exMap = new HashSet<string>();
                foreach(var ex in exArray)
                {
                    string exLow = ex.ToLower();
                    if (!exMap.Contains(exLow))
                    {
                        exMap.Add(exLow);
                    }
                }
                foreach(var file in dependences)
                {
                    string exName = Path.GetExtension(file).ToLower();
                    string exNameNoDot = exName.Replace(".", "");
                    if (exMap.Contains(exNameNoDot))
                    {
                        needFiles.Add(file);
                    }
                }
                checkFile = needFiles.ToArray();
            }
            return GetFilesMd5(checkFile);
        }

        protected string GetFileMd5(string filePath)
        {
            return GetFilesMd5(new string[] { filePath });
        }


        ////////////
        protected virtual void OnStart()
        {

        }

        protected virtual void OnEnd()
        {

        }

        protected abstract string[] OnFiles();

        protected abstract void OnOnce(string filePath);

        protected string OnMd5(string filePath)
        {
            return GetFileMd5(filePath);
        }

        protected virtual void OnProgress()
        {

        }
    }

}
