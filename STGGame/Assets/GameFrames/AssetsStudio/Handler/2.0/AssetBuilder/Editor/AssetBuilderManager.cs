using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XLibrary.Package;

namespace ASEditor
{
    public class AssetBuilderManager : Singleton<AssetBuilderManager>
    {
        //需要进行一次全局依赖Share打包
        private List<AssetBaseBuilder> m_builderCommonList = new List<AssetBaseBuilder>();
        private List<AssetBaseBuilder> m_builderCustomList = new List<AssetBaseBuilder>();

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


        public void Do()
        {
            Clear();

            Build();
        }

        public void Clear()
        {
            m_builderCommonList.Clear();
            m_builderCustomList.Clear();

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
            //
            var commonItem = AssetBuildConfiger.GetInstance().buildItemList;
            if (commonItem != null && commonItem.Count > 0)
            {
                foreach (var item in commonItem)
                {
                    if (!string.IsNullOrEmpty(item.builderName))
                    {
                        AssetBaseBuilder builder = new AssetCommonBuilder(item);
                        m_builderCommonList.Add(builder);
                    }
                }
            }

            //通过反射获取所有类
            Assembly ass = Assembly.GetAssembly(typeof(AssetCustomBuilder));
            Type[] types = ass.GetTypes();
            List<Type> builderClassList = new List<Type>();
            foreach (Type item in types)
            {
                if (item.IsAbstract) continue;

                if (item == typeof(AssetCustomBuilder))
                {
                    builderClassList.Add(item);
                }
            }

            SortedList<int, AssetBaseBuilder> sortedClassList = new SortedList<int, AssetBaseBuilder>();
            foreach (Type item in builderClassList)
            {
                object obj = Activator.CreateInstance(item);//创建一个obj对象
                AssetCustomBuilder builder = obj as AssetCustomBuilder;
                builder.Init();

                var builderPriority = builder.GetPriority();
                if (!string.IsNullOrEmpty(builder.GetName()))
                {
                    sortedClassList.Add(builderPriority, builder);
                }
            }

            for(int i = 0; i < m_builderCustomList.Count; i++)
            {
                var oldIns = m_builderCustomList[i];
                sortedClassList.Add(100000 + i, oldIns);
            }
            m_builderCustomList = new List<AssetBaseBuilder>(sortedClassList.Values);
        }

        private void Build4Common()
        {
            foreach(var builder in m_builderCommonList)
            {
                builder.Do();
            }
        }

        private void Build4Custom()
        {
            foreach (var builder in m_builderCustomList)
            {
                builder.Do();
            }

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
            bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileName;
            bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;

            var buildExportPath = AssetBuildConfiger.GetInstance().GetExportFolderPath();
            var buildPlatform = AssetBuildConfiger.GetInstance().GetBuildType();
            BuildPipeline.BuildAssetBundles(buildExportPath, GetBuildsArray(), bundleOptions, buildPlatform);//TODO:
        }

        private void BuildAfter()
        {

        }

        private AssetBundleBuild[] GetBuildsArray()
        {
            List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();
            foreach (var list in m_bundleBuilds.Values)
            {
                buildList.AddRange(list);
            }
            return buildList.ToArray();
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
