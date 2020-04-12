using System.Collections;
using System.Collections.Generic;
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
        private List<string> m_file;

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
            if (m_file == null)
            {
                m_file = new List<string>();
                var files = OnFiles();
                if (files == null || files.Length <= 0)
                    return m_file;

                m_file.AddRange(files);
            }
            return m_file;
        }

        public List<AssetBundlePair> GetBundleList()
        {
            Clear();

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
            m_file = null;
        }

        protected string[] GetReferenceds(string assetPath)
        {
            var refMap = GetRefrenceMap();
            string relaPathLow = XFileTools.GetFileRelativePath(assetPath).ToLower();
            if (refMap.ContainsKey(relaPathLow))
            {
                return refMap[relaPathLow];
            }
            return null;
        }

        protected string[] GetDependencies(string assetPath)
        {
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

                foreach (var file in fileList)
                {
                    string relativePathLow = XFileTools.GetFileRelativePath(file).ToLower();
                    string[] dps = AssetDatabase.GetDependencies(relativePathLow);
                    foreach (string path in dps)
                    {
                        string depRelatPath = XFileTools.GetFileRelativePath(path);
                        if (depRelatPath.Contains(relativePathLow))
                        {
                            if (!refSetMap.ContainsKey(relativePathLow))
                            {
                                refSetMap[relativePathLow] = new HashSet<string>();
                            }
                            var fileSet = refSetMap[relativePathLow];
                            if (!fileSet.Contains(depRelatPath))
                            {
                                fileSet.Add(path);
                            }
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
