using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XLibrary;
using XLibrary.Package;

namespace ASEditor
{
    public abstract class AssetBaseBuilder
    {
        protected string _builderName;
        private Dictionary<string, string[]> m_refMap;
        private List<string> m_files;

        public AssetBaseBuilder(string name)
        {
            _builderName = name;
        }

        public string GetName()
        {
            return _builderName;
        }

        public List<string> GetFileList()
        {
            if (m_files == null)
            {
                m_files = new List<string>();
                var files = OnFiles();
                if (files == null || files.Length <= 0)
                    return m_files;

                foreach(var fullPath in files)
                {
                    if (string.IsNullOrEmpty(fullPath))
                        continue;

                    string assetPath = XPathTools.GetRelativePath(fullPath);
                    m_files.Add(assetPath);
                }
                
            }
            return m_files;
        }

        public List<AssetBundlePair> GetBundleList()
        {
            var fileList = GetFileList();
            if (fileList.Count() <= 0)
                return null;

            List<AssetBundlePair> bundleList = new List<AssetBundlePair>();
            foreach (var file in fileList)
            {
                var bundles = OnBundles(file);
                if (bundles == null || bundles.Length <= 0)
                    continue;

                bundleList.AddRange(bundles);
            }

            return bundleList;
        }

        public List<AssetBundleBuild> GetBuildList()
        {
            var bundleList = GetBundleList();

            if (bundleList == null || bundleList.Count <= 0)
                return null;

            HashSet<string> fileSet = new HashSet<string>();
            Dictionary<string, HashSet<string>> buildMap = new Dictionary<string, HashSet<string>>();
            foreach (var pair in bundleList)
            {
                if (pair.isEmpty())
                    continue;

                var assetPath = pair.assetPath.ToLower();
                var bundleName = pair.bundleName.ToLower();

                var assetPathEx = Path.GetExtension(assetPath);
                if (assetPathEx.Contains("cs") || assetPathEx.Contains("shader"))
                    continue;

                if (fileSet.Contains(assetPath))
                    continue;
                fileSet.Add(assetPath);

                HashSet<string> bundleSet;
                if (!buildMap.TryGetValue(bundleName, out bundleSet))
                {
                    bundleSet = new HashSet<string>();
                    buildMap.Add(bundleName, bundleSet);

                }
                if (!bundleSet.Contains(assetPath))
                {
                    bundleSet.Add(assetPath);
                }
            }

            List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();
            foreach (var pair in buildMap)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = pair.Key;
                build.assetNames = pair.Value.ToArray();

                buildList.Add(build);
            }

            return buildList;
        }

        public virtual void Build()
        {
            Clear();

            var buildList = GetBuildList();
            if (buildList != null && buildList.Count > 0)
            {
                BuildAssetBundleOptions bundleOptions;//打包设置:
                bundleOptions = BuildAssetBundleOptions.None;
                bundleOptions |= BuildAssetBundleOptions.ChunkBasedCompression;
                bundleOptions |= BuildAssetBundleOptions.DeterministicAssetBundle;
                bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileName;
                bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;

                var buildExportPath = AssetBuildConfiger.GetInstance().GetExportFolderPath();
                var buildPlatform = AssetBuildConfiger.GetInstance().GetBuildType();

                if (!XFolderTools.Exists(buildExportPath))
                    XFolderTools.CreateDirectory(buildExportPath);

                BuildPipeline.BuildAssetBundles(buildExportPath, buildList.ToArray(), bundleOptions, buildPlatform);
            }
        }

        public void Clear()
        {
            m_refMap = null;
            m_files = null;
        }

        protected string[] GetReferenceds(string assetPath)
        {
            var refMap = GetRefrenceMap();
            string relaPathLow = assetPath.ToLower();
            if (refMap.ContainsKey(relaPathLow))
            {
                return refMap[relaPathLow];
            }
            return null;
        }

        protected string[] GetDependencies(string assetPath)
        {
            assetPath = XPathTools.GetRelativePath(assetPath);
            return AssetDatabase.GetDependencies(assetPath);
        }

        private Dictionary<string, string[]> GetRefrenceMap()
        {
            if (m_refMap == null)
            {
                Dictionary<string, HashSet<string>> refSetMap = new Dictionary<string, HashSet<string>>();
                var fileList = GetFileList();
                if (fileList.Count() <= 0)
                    return null;

                foreach (var assetPath in fileList)
                {
                    string[] dps = GetDependencies(assetPath);
                    foreach (string depPath in dps)
                    {
                        if (!refSetMap.ContainsKey(depPath))
                        {
                            refSetMap[depPath] = new HashSet<string>();
                        }
                        var fileSet = refSetMap[depPath];
                        if (!fileSet.Contains(assetPath))
                        {
                            fileSet.Add(assetPath);
                        }
                    }
                }

                m_refMap = new Dictionary<string, string[]>();
                foreach (var kv in refSetMap)
                {
                    m_refMap[kv.Key] = kv.Value.ToArray();
                }
            }
            return m_refMap;
        }

        protected abstract string[] OnFiles();

        protected abstract AssetBundlePair[] OnBundles(string assetPath);
    }
}
