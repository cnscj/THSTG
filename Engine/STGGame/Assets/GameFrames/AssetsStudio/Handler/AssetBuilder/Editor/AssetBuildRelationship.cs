using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ASEditor
{
    public static class AssetBuildRelationship
    {
        public static readonly string[] EMPTY_RET = new string[] { };

        static AssetDependentCollector _collector;
        public static string[] GetDependencies(string pathName)
        {
            if (AssetBuildConfiger.GetInstance().isUseDependenciesCache)
            {
                return GetCollector().GetDependencies(pathName);
            }
            else
            {
                return AssetDatabase.GetDependencies(pathName);
            }
        }
        public static string[] GetReferenceds(string pathName)
        {
            if (AssetBuildConfiger.GetInstance().isUseDependenciesCache)
            {
                return GetCollector().GetReferenceds(pathName);
            }
            else
            {
                return EMPTY_RET;
            }
        }

        [MenuItem("Assets/Refresh RelationCache")]
        public static void RefreshCache()
        {
            GetCollector().RefreshCache();
        }

        private static AssetDependentCollector GetCollector()
        {
            _collector = _collector ?? new AssetDependentCollector();
            return _collector;
        }
    }

    //TODO:加速接口优化
    //https://www.jianshu.com/p/a0ae15412a2d
    public class AssetDependentCollector
    {
        public static readonly string[] EMPTY_RET = new string[] { };
        public static readonly string CACHE_PATH = "Library/AssetRelationshipCache";

        class DependData
        {
            public int assetPathIndex;
            public Hash128 assetDependencyHash;

            public List<int> dependsPathIndex = new List<int>();
            public List<int> referencesPathIndex = new List<int>();

            // 用于返回查询结果，不保存
            public List<string> dependsPath = new List<string>();    
            public List<string> referencePath = new List<string>();
        }

        // save data
        Dictionary<string, DependData> _data;
        List<string> _strList;
        Dictionary<string, int> _strIndex;

        public AssetDependentCollector()
        {
            _data = new Dictionary<string, DependData>();
            _strList = new List<string>();
            _strIndex = new Dictionary<string, int>();
        }

        public void LoadDependsDataFile(string filePath = null)
        {
            filePath = string.IsNullOrEmpty(filePath) ? CACHE_PATH : filePath;
            //文件流的写入
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                StreamReader streamReader = new StreamReader(fileStream);
                string line = "";
                while ((line = streamReader.ReadLine()) != null)
                {

                }
                streamReader.Close();
                fileStream.Close();
            }
        }

        //将数据存储为二进制
        public void SaveDependsDataFile(string savePath = null)
        {
            savePath = string.IsNullOrEmpty(savePath) ? CACHE_PATH : savePath;
            using (FileStream fscreat = new FileStream(savePath, FileMode.CreateNew, FileAccess.Write))
            {

                fscreat.Close();
            }
        }

        //刷新缓存
        private int GetAndTryAddAssetPath(string assetPath)
        {
            var assetPathLow = assetPath.ToLower();
            if (!_strIndex.ContainsKey(assetPathLow))
            {
                var index = _strList.Count;
                _strList.Add(assetPathLow);
                _strIndex[assetPathLow] = index;
            }
            return _strIndex[assetPathLow];
        }

        private DependData GetOrCreateDependData(string assetPath)
        {
            var assetPathLow = assetPath.ToLower();
            if (!_data.ContainsKey(assetPathLow))
            {
                var index = GetAndTryAddAssetPath(assetPath);
                var depData = new DependData();
                depData.assetPathIndex = index;
                depData.assetDependencyHash = AssetDatabase.GetAssetDependencyHash(assetPath);

                _data.Add(assetPathLow, depData);
            }
            return _data[assetPathLow];
        }

        public void UpdateCache(string assetPath)
        {
            var darData = GetOrCreateDependData(assetPath);
            var dDependencyHash = AssetDatabase.GetAssetDependencyHash(assetPath);
            if (dDependencyHash != darData.assetDependencyHash)
            {
                //清除相关引用
                //清除相关依赖
                if (Hash128.Equals(dDependencyHash,0))  //文件被删
                {

                }
                else
                {

                }
            }
        }

        public void RefreshCache()
        {
            _data.Clear();
            _strList.Clear();
            _strIndex.Clear();

            var allAssetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (var srcPath in allAssetPaths)
            {
                var srcPathIndex = GetAndTryAddAssetPath(srcPath);
                var srcDepData = GetOrCreateDependData(srcPath);
                var dependencies = AssetDatabase.GetDependencies(srcPath);

                foreach (var depPath in dependencies)
                {
                    var depPathIndex = GetAndTryAddAssetPath(depPath);
                    var refDepData = GetOrCreateDependData(depPath);
                    refDepData.referencePath.Add(srcPath);
                    refDepData.referencesPathIndex.Add(srcPathIndex);

                    srcDepData.dependsPath.Add(depPath);
                    srcDepData.dependsPathIndex.Add(depPathIndex);
                }
            }
        }

        public string[] GetDependencies(string pathName)
        {
            var pathNameLow = pathName.ToLower();
            if (_data.TryGetValue(pathNameLow,  out var dependData))
            {
                return dependData.dependsPath.ToArray();
            }
            return EMPTY_RET;
        }

        public string[] GetReferenceds(string pathName)
        {
            var pathNameLow = pathName.ToLower();
            if (_data.TryGetValue(pathNameLow, out var dependData))
            {
                return dependData.referencePath.ToArray();
            }
            return EMPTY_RET;
        }
    }

}