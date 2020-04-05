using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ASGame;
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
        protected string _progresersName;
        protected Dictionary<string, FileInfo> _assetMap = new Dictionary<string, FileInfo>();
        protected HashSet<string> _checkSet = new HashSet<string>();
        private string __outputPath;

        public AssetBaseProcesser(string name)
        {
            _progresersName = name;
        }

        public virtual void Deal()
        {
            DoStart();  //用于处理公共资源

            DoAssets();

            DoEnd();
        }
        ////////////
        protected string GetFilesMd5(string[] filesPath)
        {
            if (filesPath == null || filesPath.Length <= 0)
                return "";

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
            stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);

            string finalMd5 = stringBuilder.ToString();
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

        protected string GetSaveFolderPath()
        {
            __outputPath = __outputPath ?? AssetProcesserConfiger.GetInstance().GetProcessSaveFolderPath(_progresersName);
            return __outputPath;
        }

        /////////////
        private string LoadMd5File(string srcPath)
        {
            string md5SavePath = AssetProcesserConfiger.GetInstance().GetMd5SavePath(_progresersName, srcPath);
            string md5 = "";
            if (XFileTools.Exists(md5SavePath))
            {
                md5 = File.ReadAllText(md5SavePath);
            }
            return md5;
        }

        private bool SaveMd5File(string srcPath, string md5)
        {
            if (string.IsNullOrEmpty(srcPath))
                return false;

            md5 = md5 ?? "";
            string md5SavePath = AssetProcesserConfiger.GetInstance().GetMd5SavePath(_progresersName, srcPath);
            string md5ParentPath = Path.GetDirectoryName(md5SavePath);
            if (!XFolderTools.Exists(md5ParentPath))
            {
                XFolderTools.CreateDirectory(md5ParentPath);
            }
            File.WriteAllText(md5SavePath, md5);
            return true;
        }
        private void DoStart()
        {
            string processFolderPath = AssetProcesserConfiger.GetInstance().GetProcessFloderPath();
            if (!XFolderTools.Exists(processFolderPath))
            {
                XFolderTools.CreateDirectory(processFolderPath);
            }

            OnStart();
        }
        private void DoAssets()
        {
            string[] assetFiles = OnFiles();
            if (assetFiles == null || assetFiles.Length < 0)
                return;

            List<FileInfo> procressList = new List<FileInfo>();
            foreach (var file in assetFiles)
            {
                string realPath = XFileTools.GetFileRelativePath(file); //路径做Key,有的资源可能名字相同
                string realPathLow = realPath.ToLower();

                if (_assetMap.ContainsKey(realPathLow))
                    continue;

                string recordedMd5 = LoadMd5File(realPathLow);
                string nowMd5 = OnMd5(realPathLow);

                //检测可key,这么不能区分同名不同路径的情况
                string checkKey = Path.GetFileNameWithoutExtension(realPathLow).ToLower();
                if (!_checkSet.Contains(checkKey))
                    _checkSet.Add(checkKey);

                //判断Md5,不区分大小写
                if (string.Compare(recordedMd5, nowMd5, true) == 0)
                    continue;

                FileInfo fileInfo = new FileInfo();
                fileInfo.path = realPathLow;
                fileInfo.md5 = nowMd5;

                _assetMap.Add(realPathLow, fileInfo);
            }

            foreach (var doFileInfo in procressList)
            {
                var realPathLow = doFileInfo.path;
                OnOnce(realPathLow, GetSaveFolderPath());

                //保存MD5
                if (_assetMap.TryGetValue(realPathLow, out var fileInfo))
                {
                    SaveMd5File(realPathLow, fileInfo.md5);
                }
            }
        }
        private void DoEnd()
        {
            //处理无效的MD5文件
            string md5FolderPath = AssetProcesserConfiger.GetInstance().GetMd5SaveFolderPath(_progresersName);
            bool isUseGUID = AssetProcesserConfiger.GetInstance().useGUID4SaveMd5Name;

            XFolderTools.TraverseFiles(md5FolderPath, (fullPath) =>
            {
                string fileNameNotEx = Path.GetFileNameWithoutExtension(fullPath).ToLower();
                if (isUseGUID)
                {
                    string srcPath = AssetDatabase.GUIDToAssetPath(fileNameNotEx);
                    string realPath = XFileTools.GetFileRelativePath(srcPath); //路径做Key,有的资源可能名字相同
                    string realPathLow = realPath.ToLower();
                    if (string.IsNullOrEmpty(srcPath) || !_assetMap.ContainsKey(realPathLow))
                    {
                        XFileTools.Delete(fullPath);
                    }
                }
                else
                {
                    if (!_checkSet.Contains(fileNameNotEx))
                    {
                        XFileTools.Delete(fullPath);
                    }
                }
            });

            //处理无效输出文件
            string outputFolderPath = AssetProcesserConfiger.GetInstance().GetProcessSaveFolderPath(_progresersName);
            XFolderTools.TraverseFolder(outputFolderPath, (fullPath) =>
            {
                string fileNameNotEx = Path.GetFileNameWithoutExtension(fullPath).ToLower();
                if (!_checkSet.Contains(fileNameNotEx))
                {
                    XFolderTools.DeleteDirectory(fullPath, true);
                }
            }, true);
            XFolderTools.TraverseFiles(outputFolderPath, (fullPath) =>
            {
                string fileNameNotEx = Path.GetFileNameWithoutExtension(fullPath).ToLower();
                //但凡在名字上有点关系都移除
                if (!_checkSet.Contains(fileNameNotEx))
                {
                    XFileTools.Delete(fullPath);
                }

            }, true);

            OnEnd();
        }

        ////////////
        protected virtual void OnStart(){}
        protected virtual void OnEnd(){}
        protected virtual string OnMd5(string srcFilePath)
        {
            //增量打包原则:如果只是引用的资源发生修改,但是引用资源的GUID没变,可以不用再次处理,保持引用正确即可
            return GetFileMd5(srcFilePath);
        }
        protected abstract string[] OnFiles();
        protected abstract void OnOnce(string outputFolderPath, string srcFilePath);
    }

}
