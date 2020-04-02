using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XLibrary.Package;

namespace ASEditor
{
    public class AssetBuilderManager : Singleton<AssetBuilderManager>
    {
        //需要进行一次全局依赖Share打包
        private LinkedList<KeyValuePair<string, List<AssetBundleBuild>>> m_builderList = new LinkedList<KeyValuePair<string, List<AssetBundleBuild>>>();

        private Dictionary<string, List<string>> m_shareMap = new Dictionary<string, List<string>>();
        private Dictionary<string, List<AssetBundleBuild>> m_bundleBuilds = new Dictionary<string, List<AssetBundleBuild>>();
        private Dictionary<string, int> m_refCounts = new Dictionary<string, int>();

        public void AddIntoShareMap(string builderName, string[] assetPaths)
        {
            List<string> assetList = null;
            if (!m_shareMap.TryGetValue(builderName, out assetList))
            {
                assetList = new List<string>();
                m_shareMap.Add(builderName, assetList);
            }
            assetList.AddRange(assetPaths);
        }

        public void AddIntoBuildMap(string builderName, List<AssetBundleBuild> buildList)
        {
            if (!m_bundleBuilds.ContainsKey(builderName))
            {
                m_bundleBuilds.Add(builderName,buildList);
            }
        }

        public AssetBundleBuild[] GetBuildsArray()
        {
            List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();
            foreach(var list in m_bundleBuilds.Values)
            {
                buildList.AddRange(list);
            }
            return buildList.ToArray();
        }

        public void Do()
        {
            Clear();

            Build();
        }

        public void Clear()
        {
            m_builderList.Clear();

            m_shareMap.Clear();
            m_bundleBuilds.Clear();
        }

        private void Build()
        {
            BuildBefore();
            BuildAll();
            BuildAfter();
        }

        private void BuildBefore()
        {

        }

        private void Build4Common()
        {

        }

        private void Build4Custom()
        {

        }

        private void Build4Share()
        {
            foreach(var kv in m_shareMap)
            {
                foreach(var assetPath in kv.Value)
                {
                    CollectDependencies(assetPath);
                }
            }

            foreach(var refKV in m_refCounts)
            {
                if (refKV.Value > 1)
                {

                }
            }
        }

        private void BuildAll()
        {

            Build4Common();
            Build4Custom();
            Build4Share();

            BuildAssetBundleOptions bundleOptions;//打包设置:
            bundleOptions = BuildAssetBundleOptions.None;
            bundleOptions |= BuildAssetBundleOptions.ChunkBasedCompression;
            bundleOptions |= BuildAssetBundleOptions.DeterministicAssetBundle;
            bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileName;  //没有扩展的加载直接禁用了
            bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;

            var buildExportPath = AssetBuildConfiger.GetInstance().GetExportFolderPath();
            var buildPlatform = AssetBuildConfiger.GetInstance().GetBuildType();
            BuildPipeline.BuildAssetBundles(buildExportPath, GetBuildsArray(), bundleOptions, buildPlatform);//TODO:
        }

        private void BuildAfter()
        {

        }

        private void CollectDependencies(string assetPath)
        {
            string[] dps = AssetDatabase.GetDependencies(assetPath);
            foreach (var dp in dps)
            {
                if (m_refCounts.TryGetValue(dp, out var refCount))
                {
                    m_refCounts[dp] = refCount + 1;
                }
                else
                {
                    m_refCounts.Add(dp, 1);
                }
            }
        }
    }

}
