using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ASEditor
{
    public static class AssetBuildRelationship
    {
        static AssetDependentCollector _collector;
        public static string[] GetDependencies(string pathName)
        {
            return GetCollector().GetDependencies(pathName);
        }
        public static string[] GetReferenceds(string pathName)
        {
            return GetCollector().GetReferenceds(pathName);
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
        public static readonly string CACHE_PATH = "Library/AssetRelationshipCache";

        class DependData
        {
            public int assetPathIndex;
            public Hash128 assetDependencyHash;
            public int[] dependsPathIndex;
            public string[] dependsPath; // 用于返回查询结果，不保存
        }

        // save data
        Dictionary<string, DependData> _data;
        List<string> _strList;
        Dictionary<string, int> _strIndex;


        public void LoadDependsDataFile(string filePath = null)
        {
            filePath = string.IsNullOrEmpty(filePath) ? CACHE_PATH : filePath;

        }

        //将数据存储为二进制
        public void SaveDependsDataFile(string savePath = null)
        {
            savePath = string.IsNullOrEmpty(savePath) ? CACHE_PATH : savePath;

        }

        //刷新缓存
        public void RefreshCache()
        {
            var allAssetPaths = AssetDatabase.GetAllAssetPaths();
            foreach(var assetPath in allAssetPaths)
            {
                var dependencies = AssetDatabase.GetDependencies(assetPath);
                foreach(var depPath in dependencies)
                {

                }
            }
        }

        public string[] GetDependencies(string pathName)
        {
            pathName = pathName.ToLower();
            if (AssetBuildConfiger.GetInstance().isUseDependenciesCache)
            {
                return null;
            }
            else
            {
                return AssetDatabase.GetDependencies(pathName);
            }

        }
        public string[] GetReferenceds(string pathName)
        {
            pathName = pathName.ToLower();
            if (AssetBuildConfiger.GetInstance().isUseDependenciesCache)
            {
                return null;
            }
            else
            {
                return new string[] { };
            }
        }
    }

}