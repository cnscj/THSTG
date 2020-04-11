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
        protected Dictionary<string, string[]> m_refMap;

        public AssetBaseBuilder(string name)
        {
            _builderName = name;
        }

        public string GetName()
        {
            return _builderName;
        }

        public virtual void Build()
        {
            Clear();

            //var list = GetBuildList();
            //if (list.Count > 0)
            //{
            //    BuildAssetBundleOptions bundleOptions;//打包设置:
            //    bundleOptions = BuildAssetBundleOptions.None;
            //    bundleOptions |= BuildAssetBundleOptions.ChunkBasedCompression;
            //    bundleOptions |= BuildAssetBundleOptions.DeterministicAssetBundle;
            //    bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileName;
            //    bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;

            //    var buildExportPath = AssetBuildConfiger.GetInstance().GetExportFolderPath();
            //    var buildPlatform = AssetBuildConfiger.GetInstance().GetBuildType();

            //    if (!XFolderTools.Exists(buildExportPath))
            //        XFolderTools.CreateDirectory(buildExportPath);

            //    BuildPipeline.BuildAssetBundles(buildExportPath, list.ToArray(), bundleOptions, buildPlatform);
            //}
        }
        public virtual void Deal()
        {
            Clear();

           
        }

        public void Clear()
        {
            m_refMap = null;
        }

        private void DoAsset()
        {
            var files = OnFiles();
            if (files == null || files.Length <= 0)
                return;

            var bundles = OnBundles(files);
            if (bundles == null || bundles.Length <= 0)
                return;

            Dictionary<string, string> bundleMap = new Dictionary<string, string>();
            foreach(var pair in bundles)
            {
                if (bundleMap.ContainsKey(pair.assetPath))
                {
                    bundleMap.Add(pair.assetPath, pair.bundleName);
                }
            }
            //TODO:
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
                string[] files = OnFiles();
                if (files == null || files.Length <= 0)
                    return null;

                foreach (var file in files)
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

        protected abstract AssetBundlePair[] OnBundles(string[] files);
    }
}
