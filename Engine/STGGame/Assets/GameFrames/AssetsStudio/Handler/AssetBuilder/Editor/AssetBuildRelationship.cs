using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using XLibrary;
using XLibrary.Package;

namespace ASEditor
{
    public static class AssetBuildRelationship
    {
        public static readonly string[] EMPTY_RET = new string[] { };

        public static string[] GetDependencies(string path)
        {
            if (AssetBuildConfiger.GetInstance().isUseDependenciesCache)
            {
                TryUpdate(path);
                return GetCollector().GetDependencies(path);
            }
            else
            {
                return AssetDatabase.GetDependencies(path);
            }
        }

        public static string[] GetReferences(string path)
        {
            if (AssetBuildConfiger.GetInstance().isUseDependenciesCache)
            {
                TryUpdate(path);
                return GetCollector().GetReferences(path);
            }
            else
            {
                return EMPTY_RET;
            }
        }

        public static void RefreshCache()
        {
            GetCollector().RefreshCache();
        }

        public static void SaveCache()
        {
            GetCollector().SaveDependsDataFile();
        }

        public static void TryLoadCache()
        {
            GetCollector().TryLoadDependsDataFile();
        }
        public static void LoadCache()
        {
            GetCollector().LoadDependsDataFile();
        }

        private static void TryUpdate(string path)
        {
            GetCollector().LoadOrSaveDependsDataFile();
            GetCollector().UpdateRelation(path);
        }

        private static AssetDependentCollector GetCollector()
        {
            return AssetDependentCollector.GetInstance();
        }
    }

    //加速接口优化
    //https://www.jianshu.com/p/a0ae15412a2d
    public class AssetDependentCollector : Singleton<AssetDependentCollector>
    {
        public static readonly int VERSION = 100;
        public static readonly string[] EMPTY_RET = new string[] { };
        public static readonly string EMPTY_CODE = "00000000000000000000000000000000";
        public static readonly string CACHE_PATH = "Library/AssetRelationshipCache";
        public static readonly HashSet<string> ROOT_PATHS = new HashSet<string> { "Assets" };

        [SerializeField]
        class RelationData
        {
            public int index;
            public string hash;
            public int[] dependsPathIndex;
        }

        [SerializeField]
        class FileData
        {
            public int version;
            public long date;

            public int assetCount;
            public int assetDependCount;
            public string[] assetPaths;
            public RelationData[] assetDepends;
        }

        class CollectionData
        {
            public int assetPathIndex;
            public string assetDependencyHash;

            //下面的可以考虑换成Dictionary
            public List<int> dependsPathIndex = new List<int>();
            public List<int> referencesPathIndex = new List<int>();

            // 用于返回查询结果，不保存
            public List<string> dependsPath = new List<string>();
            public List<string> referencePath = new List<string>();
        }

        List<string> _strList;
        Dictionary<string, CollectionData> _data;
        Dictionary<string, int> _strIndex;

        public bool IsLoaded { get; private set; }
        public bool IsSaved { get; private set; }

        public AssetDependentCollector()
        {
            _data = new Dictionary<string, CollectionData>();
            _strList = new List<string>();
            _strIndex = new Dictionary<string, int>();
        }

        public string[] GetDependencies(string pathName)
        {
            var pathNameLow = pathName.ToLower();
            if (_data.TryGetValue(pathNameLow, out var dependData))
            {
                return dependData.dependsPath.ToArray();
            }
            return new string[] { pathName };
        }

        public string[] GetReferences(string pathName)
        {
            var pathNameLow = pathName.ToLower();
            if (_data.TryGetValue(pathNameLow, out var dependData))
            {
                return dependData.referencePath.ToArray();
            }
            return new string[] { pathName };
        }

        public void LoadOrSaveDependsDataFile(string filePath = null)
        {
            filePath = string.IsNullOrEmpty(filePath) ? CACHE_PATH : filePath;
            if (!IsLoaded)
            {
                RefreshCache();
                SaveDependsDataFile(filePath);
                IsLoaded = true;
            }
        }

        public void TryLoadDependsDataFile(string filePath = null)
        {
            filePath = string.IsNullOrEmpty(filePath) ? CACHE_PATH : filePath;
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                LoadOrSaveDependsDataFile();
            }
            else
            {
                LoadDependsDataFile();
            }
        }

        public void LoadDependsDataFile(string filePath = null)
        {
            filePath = string.IsNullOrEmpty(filePath) ? CACHE_PATH : filePath;

            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
                return;

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream);

            var fileData = new FileData();
            string line = string.Empty;

            //第一行
            line = streamReader.ReadLine().Trim();
            string[] headSections = line.Split(',');
            fileData.version = int.Parse(headSections[0]);
            fileData.date = long.Parse(headSections[1]);

            //第二行
            line = streamReader.ReadLine().Trim();
            string[] lengthSections = line.Split(',');
            fileData.assetCount = int.Parse(lengthSections[0]);
            fileData.assetDependCount = int.Parse(lengthSections[1]);

            fileData.assetPaths = new string[fileData.assetCount];
            for (int i = 0; i < fileData.assetCount; i++)
            {
                line = streamReader.ReadLine().Trim();
                string[] assetSections = line.Split('=');

                var assetIndex = int.Parse(assetSections[0]);
                fileData.assetPaths[assetIndex] = assetSections[1];
            }

            fileData.assetDepends = new RelationData[fileData.assetDependCount];
            for (int i = 0; i < fileData.assetDependCount; i++)
            {
                line = streamReader.ReadLine().Trim();
                int leftBracketIndex = line.IndexOf('(',0);
                int rightBracketIndex = line.IndexOf(')', leftBracketIndex);
                int colonIndex = line.IndexOf(':', rightBracketIndex);

                var indexStr = line.Substring(0, leftBracketIndex);
                var hashCode = line.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1);
                var refStr = line.Substring(colonIndex + 1);

                var index = int.Parse(indexStr);
                fileData.assetDepends[index] = fileData.assetDepends[index] ?? new RelationData();
                var relationData = fileData.assetDepends[index];

                var refSections = refStr.Split(',');

                relationData.index = index;
                relationData.hash = hashCode;

                relationData.dependsPathIndex = new int[refSections.Length];
                for (int j = 0; j < refSections.Length; j++)
                    relationData.dependsPathIndex[j] = int.Parse(refSections[j]);

            }

            streamReader.Close();
            fileStream.Close();

            streamReader.Dispose();
            fileStream.Dispose();

            //转换
            _data.Clear();
            _strList.Clear();
            _strIndex.Clear();

            for(int i = 0; i < fileData.assetPaths.Length; i++)
            {
                _strList.Add(fileData.assetPaths[i]);
                _strIndex[_strList[i].ToLower()] = i;
            }

            
            for (int i = 0; i < fileData.assetDepends.Length; i++)
            {
                var depData = fileData.assetDepends[i];
                var srcPath = _strList[depData.index];
                var srcCollectionData = GetOrCreateDependData(srcPath.ToLower());

                for (int j = 0; j < depData.dependsPathIndex.Length ;j++)
                {
                    var depPath = _strList[depData.dependsPathIndex[j]];
                    var depCollectionData = GetOrCreateDependData(depPath.ToLower());

                    srcCollectionData.dependsPath.Add(depPath);
                    srcCollectionData.dependsPathIndex.Add(depData.dependsPathIndex[j]);

                    depCollectionData.referencePath.Add(srcPath);
                    depCollectionData.referencesPathIndex.Add(depData.index);
                }
            }
            IsSaved = true;
        }

        //将数据存储为二进制
        public void SaveDependsDataFile(string savePath = null)
        {
            savePath = string.IsNullOrEmpty(savePath) ? CACHE_PATH : savePath;
            FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);

            //转换
            FileData fileData = new FileData();
            fileData.version = VERSION;
            fileData.date = XTimeTools.NowTimeStampMs;
            fileData.assetPaths = _strList.ToArray();

            var assetDependList = new List<RelationData>();
            foreach(var depData in _data.Values)
            {
                var relationData = new RelationData();
                relationData.index = depData.assetPathIndex;
                relationData.hash = depData.assetDependencyHash;

                relationData.dependsPathIndex = depData.dependsPathIndex.ToArray();

                assetDependList.Add(relationData);
            }
            fileData.assetDepends = assetDependList.ToArray();
            fileData.assetCount = _strList.Count;
            fileData.assetDependCount = assetDependList.Count;

            /////
            
            StringBuilder indeBuilder = new StringBuilder();
            streamWriter.WriteLine(string.Format("{0},{1}", fileData.version, fileData.date));
            streamWriter.WriteLine(string.Format("{0},{1}", fileData.assetCount, fileData.assetDependCount));

            for (int i = 0; i < fileData.assetCount; i++)
            {
                streamWriter.WriteLine(string.Format("{0}={1}", i, fileData.assetPaths[i]));
            }

            for (int i = 0; i < fileData.assetDependCount; i++)
            {
                var assetDepend = fileData.assetDepends[i];
                indeBuilder.Clear();
                for (int j = 0; j < assetDepend.dependsPathIndex.Length; j++)
                {
                    indeBuilder.Append(assetDepend.dependsPathIndex[j]);
                    if (j < assetDepend.dependsPathIndex.Length - 1) indeBuilder.Append(",");
                }
                var depStr = indeBuilder.ToString();

                streamWriter.WriteLine(string.Format("{0}({1}):{2}", assetDepend.index, assetDepend.hash, depStr));
            }

            streamWriter.Close();
            fileStream.Close();

            streamWriter.Dispose();
            fileStream.Dispose();

            IsSaved = true;
        }

        private int GetAndTryAddAssetPath(string assetPath)
        {
            var assetPathLow = assetPath.ToLower();
            if (!_strIndex.ContainsKey(assetPathLow))
            {
                var index = _strList.Count;
                _strList.Add(assetPath);
                _strIndex[assetPathLow] = index;
            }
            return _strIndex[assetPathLow];
        }

        private CollectionData GetOrCreateDependData(string assetPath)
        {
            var assetPathLow = assetPath.ToLower();
            if (!_data.ContainsKey(assetPathLow))
            {
                var index = GetAndTryAddAssetPath(assetPath);
                var depData = new CollectionData();
                depData.assetPathIndex = index;
                depData.assetDependencyHash = AssetDatabase.GetAssetDependencyHash(assetPath).ToString();

                _data.Add(assetPathLow, depData);
            }
            return _data[assetPathLow];
        }

        public void RefreshCache()
        {
            _data.Clear();
            _strList.Clear();
            _strIndex.Clear();

            var allAssetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (var srcPath in allAssetPaths)
            {
                AddRelation(srcPath);
            }
        }

        public void RemoveRelation(string srcPath)
        {
            var srcPathLow = srcPath.ToLower();
            if (_data.TryGetValue(srcPathLow, out var srcDepData))
            {
                //但是_strIndex不能做删除,只能做新增,否则index全数作废
                //从其他人的依赖中引用中移除自己
                var srcPathIndex = _strIndex[srcPathLow];
                foreach(var refPathIndex in srcDepData.referencesPathIndex)
                {
                    var refPath = _strList[refPathIndex];
                    var refPathLow = refPath.ToLower();
                    if (_data.TryGetValue(refPathLow, out var refDepData))
                    {
                        refDepData.dependsPath.Remove(srcPath);
                        refDepData.dependsPathIndex.Remove(srcPathIndex);
                    }
                }

                _strIndex.Remove(srcPathLow);
                _strList[srcPathIndex] = "";    //这里用空值占位

                _data.Remove(srcPathLow);
            }

        }

        public void MoveRelation(string formPath,string destPath)
        {
            var formPathLow = formPath.ToLower();
            var destPathLow = destPath.ToLower();
            if (_data.TryGetValue(formPathLow, out var depData))
            {
                var pathIndex = depData.assetPathIndex;
                _strList[pathIndex] = destPath;
                depData.dependsPath.Remove(formPath);
                depData.referencePath.Remove(formPath);

                depData.dependsPath.Add(destPath);
                depData.referencePath.Add(destPath);

                _data.Remove(formPathLow);
                _data.Add(destPathLow, depData);
                
            }
        }

        public void UpdateRelation(string srcPath)
        {
            var srcPathLow = srcPath.ToLower();
            if (_data.TryGetValue(srcPathLow,out var srcDepData))
            {
                var oldHashCode = srcDepData.assetDependencyHash;
                var newHashCode = AssetDatabase.GetAssetDependencyHash(srcPath).ToString();
                if (string.Compare(oldHashCode, newHashCode) == 0)
                {
                    return;
                }

                if (string.Compare(newHashCode, EMPTY_CODE) == 0)
                {
                    RemoveRelation(srcPath);
                    return;
                }
                else
                {
                    //更新
                    var srcPathIndex = _strIndex[srcPathLow];
                    srcDepData.referencePath.Remove(srcPath);
                    srcDepData.referencesPathIndex.Remove(srcPathIndex);
                    srcDepData.dependsPath.Clear();
                    srcDepData.dependsPathIndex.Clear();

                    var dependencies = AssetDatabase.GetDependencies(srcPath);
                    foreach (var depPath in dependencies)
                    {
                        var depPathIndex = GetAndTryAddAssetPath(depPath);
                        var refDepData = GetOrCreateDependData(depPath);
                        srcDepData.dependsPath.Add(depPath);
                        srcDepData.dependsPathIndex.Add(depPathIndex);

                        refDepData.referencePath.Add(srcPath);
                        refDepData.referencesPathIndex.Add(srcPathIndex);
                    }
                }
            }
            else
            {
                AddRelation(srcPath);
            }
        }

        public void AddRelation(string srcPath)
        {
            if (!IsPathEligible(srcPath))
                return;

            var srcHashCode = AssetDatabase.GetAssetDependencyHash(srcPath).ToString();
            if (string.Compare(srcHashCode, EMPTY_CODE) == 0)
                return;

            var dependencies = AssetDatabase.GetDependencies(srcPath);
            var srcPathIndex = GetAndTryAddAssetPath(srcPath);
            var srcDepData = GetOrCreateDependData(srcPath);
            foreach (var depPath in dependencies)
            {
                if (!IsPathEligible(depPath))
                    continue;

                var depHashCode = AssetDatabase.GetAssetDependencyHash(depPath).ToString();
                if (string.Compare(depHashCode, EMPTY_CODE) == 0)
                    continue;

                var depPathIndex = GetAndTryAddAssetPath(depPath);
                var refDepData = GetOrCreateDependData(depPath);
                srcDepData.dependsPath.Add(depPath);
                srcDepData.dependsPathIndex.Add(depPathIndex);

                refDepData.referencePath.Add(srcPath);
                refDepData.referencesPathIndex.Add(srcPathIndex);
            }
        }

        private bool IsPathEligible(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (ROOT_PATHS.Count <= 0)
                return true;

            int left = path.IndexOf('/');
            if (left < 0)
                return false;

            var rootPath = path.Substring(0,left);
 
            return ROOT_PATHS.Contains(rootPath);
        }
    }

    public class AssetDependentCollectorPostprocessor : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (!AssetBuildConfiger.GetInstance().isUseDependenciesCache)
                return;

            if (!AssetDependentCollector.GetInstance().IsLoaded)
                return;

            foreach (var assetPath in importedAssets)
                AssetDependentCollector.GetInstance().AddRelation(assetPath);
            foreach (var assetPath in deletedAssets)
                AssetDependentCollector.GetInstance().RemoveRelation(assetPath);
            for(int i = 0; i < movedAssets.Length ; i++)
            {
                var srcPath = movedFromAssetPaths[i];
                var destPath = movedAssets[i];
                AssetDependentCollector.GetInstance().MoveRelation(srcPath, destPath);
            }

        }
    }

}