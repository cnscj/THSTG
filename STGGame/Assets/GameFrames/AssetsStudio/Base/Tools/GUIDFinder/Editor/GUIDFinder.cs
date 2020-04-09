 
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using XLibrary;

namespace ASGame
{
    public static class GUIDFinder
    {
        public static string[] SERIALIZABLE_FILE_SUFFIX = { ".prefab", ".unity", ".mat", ".asset", ".controller", ".playable" };
        public static Dictionary<string,string[]> GetAllReferences(string searchPath = null, string[] searchSuffix = null)
        {
            searchPath = string.IsNullOrEmpty(searchPath) ? Application.dataPath : searchPath;
            searchSuffix = searchSuffix != null ? searchSuffix : SERIALIZABLE_FILE_SUFFIX;
            Dictionary<string, HashSet<string>> refSetMap = new Dictionary<string, HashSet<string>>();

            string[] files = Directory.GetFiles(searchPath, "*.*", SearchOption.AllDirectories)
                   .Where(s => searchSuffix.Contains(Path.GetExtension(s).ToLower()))
                   .ToArray();

            foreach (var file in files)
            {
                string relativePath = XFileTools.GetFileRelativePath(file);
                string[] dps = AssetDatabase.GetDependencies(relativePath);
                foreach (string path in dps)
                {
                    string depRelatPath = XFileTools.GetFileRelativePath(path);
                    if (depRelatPath.Contains(relativePath))
                    {
                        if (!refSetMap.ContainsKey(relativePath))
                        {
                            refSetMap[relativePath] = new HashSet<string>();
                        }
                        var fileSet = refSetMap[relativePath];
                        if (!fileSet.Contains(depRelatPath))
                        {
                            fileSet.Add(path);
                        }
                    }
                }
            }

            Dictionary<string, string[]> refMap = new Dictionary<string, string[]>();
            foreach (var kv in refSetMap)
            {
                refMap[kv.Key] = kv.Value.ToArray();
            }

            return refMap;
        }
        public static string[] FindReferences(string assetPath, string searchPath = null, string[] searchSuffix = null)
        {
            searchPath = string.IsNullOrEmpty(searchPath) ? Application.dataPath : searchPath;
            searchSuffix = searchSuffix != null ? searchSuffix : SERIALIZABLE_FILE_SUFFIX;
            string assetPathLow = XFileTools.GetFileRelativePath(assetPath.ToLower());

            string[] files = Directory.GetFiles(searchPath, "*.*", SearchOption.AllDirectories)
                   .Where(s => searchSuffix.Contains(Path.GetExtension(s).ToLower()))
                   .ToArray();


            HashSet<string> refDict = new HashSet<string>();
            foreach(var file in files)
            {
                string relativePath = XFileTools.GetFileRelativePath(file);
                string[] dps = AssetDatabase.GetDependencies(relativePath);
                foreach (string path in dps)
                {
                    string pathLow = path.ToLower();
                    if (pathLow.Contains(assetPathLow))
                    {
                        if (!refDict.Contains(path))
                        {
                            refDict.Add(path);
                        }
                    }
                }
            }

            return refDict.ToArray();
        }

        public static string[] FindDependencies(string assetPath, bool recursive = false)
        {
            string path = XFileTools.GetFileRelativePath(assetPath);
            return AssetDatabase.GetDependencies(path, recursive);
        }

    }
}
