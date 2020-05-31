using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using XLibrary;
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
            public AssetProcessCheckfile checkfile;
        }
        protected string _progresersName;
        protected Dictionary<string, FileInfo> _assetMap = new Dictionary<string, FileInfo>();
        protected HashSet<string> _checkSet = new HashSet<string>();
        private string __outputPath;

        public AssetBaseProcesser(string name)
        {
            _progresersName = name;
        }

        public string GetName()
        {
            return _progresersName;
        }

        public virtual void Deal()
        {
            DoStart();  //用于处理公共资源

            DoAssets();

            DoEnd();    //存在误伤的情况
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
                string md5 = XFileTools.GetMD5(filePath);
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
        private AssetProcessCheckfile LoadCheckfile(string srcPath)
        {
            //没有或读取有问题则创建
            AssetProcessCheckfile checkfile;
            string checkfileSavePath = AssetProcesserConfiger.GetInstance().GetCheckfileSavePath(_progresersName, srcPath, ".asset");
            if (XFileTools.Exists(checkfileSavePath))
            {
                var srcAsset = AssetDatabase.LoadAssetAtPath<AssetProcessCheckfile>(checkfileSavePath); //脚本丢失可能造成信息丢失
                if (srcAsset == null)
                {
                    checkfile = ScriptableObject.CreateInstance<AssetProcessCheckfile>();
                }
                else
                {
                    checkfile = Object.Instantiate(srcAsset);
                }
            }
            else
            {
                checkfile = ScriptableObject.CreateInstance<AssetProcessCheckfile>();
            }

            return checkfile;
        }

        private bool SaveCheckfile(string srcPath, AssetProcessCheckfile checkfile)
        {
            if (checkfile == null)
                return false;

            string checkfileSavePath = AssetProcesserConfiger.GetInstance().GetCheckfileSavePath(_progresersName, srcPath, ".asset");
            var outName = OnCheckFileName(srcPath);
            if (!string.IsNullOrEmpty(outName))
            {
                string exName = Path.GetExtension(checkfileSavePath);
                string parentPath = Path.GetDirectoryName(checkfileSavePath);
                checkfileSavePath = XPathTools.Combine(parentPath, string.Format("{0}{1}", outName, exName));
            }

            if (!XFileTools.Exists(checkfileSavePath))
            {
                string checkfileParentPath = Path.GetDirectoryName(checkfileSavePath);
                if (!XFolderTools.Exists(checkfileParentPath))
                {
                    XFolderTools.CreateDirectory(checkfileParentPath);
                }
            }
            AssetDatabase.DeleteAsset(checkfileSavePath);
            AssetDatabase.CreateAsset(checkfile, checkfileSavePath);

            return true;
        }


        private void DoStart()
        {
            string processFolderPath = AssetProcesserConfiger.GetInstance().GetProcessFloderPath();
            if (!XFolderTools.Exists(processFolderPath))
            {
                XFolderTools.CreateDirectory(processFolderPath);
            }

            string outputPath = AssetProcesserConfiger.GetInstance().GetProcessSaveFolderPath(_progresersName);
            if (!XFolderTools.Exists(outputPath))
            {
                XFolderTools.CreateDirectory(outputPath);
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
                if (string.IsNullOrEmpty(file))
                    continue;

                string realPath = XPathTools.GetRelativePath(file); //路径做Key,有的资源可能名字相同
                realPath = XPathTools.NormalizePath(realPath);

                if (_assetMap.ContainsKey(realPath))
                    continue;

                //如果只检测key,不能区分同名不同路径的情况(或者前缀相同,后缀不同
                string checkKey1 = Path.GetFileNameWithoutExtension(realPath);
                string checkKey2 = XStringTools.SplitPathKey(realPath);
                if (!_checkSet.Contains(checkKey1))
                    _checkSet.Add(checkKey1);
                if (!_checkSet.Contains(checkKey2))
                    _checkSet.Add(checkKey2);

                AssetProcessCheckfile checkfile = LoadCheckfile(realPath);
                if (!OnCheck(realPath, checkfile))   //如果生成的文件没了,也应该重新生成
                {
                    continue;
                }  

                FileInfo fileInfo = new FileInfo();
                fileInfo.path = realPath;
                fileInfo.checkfile = checkfile;

                _assetMap.Add(realPath, fileInfo);
                procressList.Add(fileInfo);
            }

            foreach (var doFileInfo in procressList)
            {
                var realPath = doFileInfo.path;
                OnOnce(realPath);

                //保存Checkfile
                if (_assetMap.TryGetValue(realPath, out var fileInfo))
                {
                    var newCheckfile = OnUpdate(fileInfo.path, fileInfo.checkfile);
                    SaveCheckfile(fileInfo.path, newCheckfile);
                }
            }
        }
        
        private void DoEnd()
        {
            //处理无效的Checkfile文件
            string checkfileFolderPath = AssetProcesserConfiger.GetInstance().GetCheckfileSaveFolderPath(_progresersName);
            bool isUseGUID = AssetProcesserConfiger.GetInstance().useGUID4SaveCheckfileName;

            XFolderTools.TraverseFiles(checkfileFolderPath, (fullPath) =>
            {
                string exName = Path.GetExtension(fullPath).ToLower();
                if (exName.Contains("meta")) return;

                string realPath = XPathTools.GetRelativePath(fullPath); //路径做Key,有的资源可能名字相同
                string realPathLow = realPath.ToLower();
                if (isUseGUID)
                {
                    string srcPath = AssetDatabase.GUIDToAssetPath(realPath);
                    
                    if (string.IsNullOrEmpty(srcPath) || !_assetMap.ContainsKey(realPathLow))
                    {
                        XFileTools.Delete(fullPath);
                    }
                }
                else
                {
                    string checkKey1 = Path.GetFileNameWithoutExtension(realPath);
                    string checkKey2 = XStringTools.SplitPathKey(realPath);
                    if (!_checkSet.Contains(checkKey1) && !_checkSet.Contains(checkKey2))
                    {
                        XFileTools.Delete(fullPath);
                    }
                }
            });

            //处理无效输出文件
            string outputFolderPath = AssetProcesserConfiger.GetInstance().GetProcessSaveFolderPath(_progresersName);
            XFolderTools.TraverseFiles(outputFolderPath, (fullPath) =>
            {
                string checkKey1 = Path.GetFileNameWithoutExtension(fullPath);
                string checkKey2 = XStringTools.SplitPathKey(fullPath);
                //但凡在名字上有点关系都移除,多重key检测
                if (!_checkSet.Contains(checkKey1) && !_checkSet.Contains(checkKey2))
                {
                    XFileTools.Delete(fullPath);
                }

            }, true);

            OnEnd();
        }

        ////////////
        protected virtual void OnStart(){}
        protected virtual void OnEnd(){}

        //为false时重新生成
        protected virtual bool OnCheck(string srcFilePath , AssetProcessCheckfile checkfile)//对指纹文件进行检查
        {
            bool ret = true;
            string recordedMd5 = checkfile.md5;
            string nowMd5 = GetFileMd5(srcFilePath);

            //判断Md5,不区分大小写
            if (string.Compare(recordedMd5, nowMd5, true) == 0)
                ret = false;

            return ret;
        }
        protected virtual AssetProcessCheckfile OnUpdate(string srcFilePath, AssetProcessCheckfile checkfile)
        {
            string nowMd5 = GetFileMd5(srcFilePath);
            checkfile.md5 = nowMd5;

            return checkfile;
        }

        protected virtual string OnCheckFileName(string srcFilePath)
        {
            return null;
        }

        //所有待处理文件路径
        protected abstract string[] OnFiles();

        //
        protected abstract void OnOnce(string srcFilePath);
    }

}
