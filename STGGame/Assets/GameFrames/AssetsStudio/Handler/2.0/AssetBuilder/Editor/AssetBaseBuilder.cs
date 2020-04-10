using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XLibrary;
using XLibrary.Package;

namespace ASEditor
{
    public abstract class AssetBaseBuilder
    {
        protected class Result
        {
            public List<string> assetPaths;
            public List<AssetBundleBuild> assetBuilds;
        }
        protected string _builderName;
        public AssetBaseBuilder(string name)
        {
            _builderName = name;
        }

        public string GetName()
        {
            return _builderName;
        }

        public virtual List<AssetBundleBuild> GetBuildList()
        {
            var result = Do();
            return result.assetBuilds;
        }

        public virtual void Build()
        {
            var list = GetBuildList();
            if (list.Count > 0)
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

                BuildPipeline.BuildAssetBundles(buildExportPath, list.ToArray(), bundleOptions, buildPlatform);
            }
        }
        public virtual void Deal()
        {
            var result = Do();

            //送入预打包队列
            AssetBuilderManager.GetInstance().AddIntoBuildMap(_builderName, result.assetBuilds);

            //全局依收集
            AssetBuilderManager.GetInstance().AddIntoShareMap(_builderName, result.assetPaths);
        }

        protected Result Do()
        {
            Result result = new Result();
            DoStart();

            DoAssets(result);

            DoEnd();
            return result;
        }

        private void DoStart()
        {
            OnStart();
        }

        private void DoAssets(Result result)
        {
            Dictionary<string, string> fileMap = new Dictionary<string, string>();
            Dictionary<string, List<string>> buildMap = new Dictionary<string, List<string>>();
            string[] assetFiles = OnFiles();
            if (assetFiles == null || assetFiles.Length < 0)
                return;

            OnBefore(assetFiles);
            var fileList = new List<string>(assetFiles);
            foreach (var assetPath in fileList)
            {
                string realPath = XFileTools.GetFileRelativePath(assetPath);
                string realPathLow = realPath.ToLower();

                if (fileMap.ContainsKey(realPathLow))
                    continue;

                string bundleName = OnName(realPathLow);
                if (string.IsNullOrEmpty(bundleName))
                    continue;

                bundleName = bundleName.ToLower();
                fileMap.Add(realPathLow,bundleName);

                List<string> assetList = null;
                if (!buildMap.TryGetValue(bundleName, out assetList))
                {
                    assetList = new List<string>();
                    buildMap.Add(bundleName, assetList);
                }
                assetList.Add(realPathLow);
            }

            List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();
            foreach (var kv in buildMap)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = kv.Key;
                build.assetNames = kv.Value.ToArray();

                buildList.Add(build);
            }

            result.assetPaths = fileList;
            result.assetBuilds = buildList;

            OnAfter(result);
        }

        private void DoEnd()
        {
            OnEnd();
        }

        protected virtual void OnStart() { }
        protected virtual void OnEnd() { }
        protected abstract string[] OnFiles();
        protected virtual void OnBefore(string[] files) { }
        protected abstract string OnName(string assetPath);
        protected virtual void OnAfter(Result result) { }
    }
}
