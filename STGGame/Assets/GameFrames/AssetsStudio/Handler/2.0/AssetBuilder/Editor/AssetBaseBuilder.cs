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
        protected string _builderName;
        protected Dictionary<string, string> m_buildMap = new Dictionary<string, string>();
        protected Dictionary<string, List<string>> m_buildList = new Dictionary<string, List<string>>();

        public AssetBaseBuilder(string name)
        {
            _builderName = name;
        }

        public string GetName()
        {
            return _builderName;
        }

        public void Do()
        {
            DoStart();

            DoAssets();

            DoEnd();
        }

        private void DoStart()
        {
            OnStart();
        }

        private void DoAssets()
        {
            string[] assetFiles = OnFiles();
            if (assetFiles == null || assetFiles.Length < 0)
                return;

            //设置 bundle
            foreach (var assetPath in assetFiles)
            {
                string realPath = XFileTools.GetFileRelativePath(assetPath);
                string realPathLow = realPath.ToLower();

                if (m_buildMap.ContainsKey(realPathLow))
                    continue;

                string bundleName = OnName(assetPath);
                if (string.IsNullOrEmpty(bundleName))
                    continue;

                bundleName = bundleName.ToLower();

                List<string> assetList = null;
                if (!m_buildList.TryGetValue(bundleName, out assetList))
                {
                    assetList = new List<string>();
                    m_buildList.Add(bundleName,assetList);
                }
                assetList.Add(assetPath);
            }
            var buildMap = OnBuilds();

            //送入预打包队列
            AssetBuilderManager.GetInstance().AddIntoBuildMap(_builderName, buildMap);

            //全局依收集
            AssetBuilderManager.GetInstance().AddIntoShareMap(_builderName, assetFiles);
        }

        private void DoEnd()
        {
            OnEnd();
        }

        protected virtual List<AssetBundleBuild> OnBuilds()
        {
            List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();
            foreach(var kv in m_buildList)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = kv.Key;
                build.assetNames = kv.Value.ToArray();

                buildList.Add(build);
            }
            return buildList;
        }

        protected virtual void OnStart() { }
        protected virtual void OnEnd() { }
        protected abstract string[] OnFiles();
        protected abstract string OnName(string assetPath);

    }
}
