using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XLibrary;
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

        private List<KeyValuePair<string, string>> m_buildList = new List<KeyValuePair<string, string>>();

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

        public void AddIntoBuildList(string assetPath, string builderName)
        {
        }


        public void Do(AssetBaseBuilder []builders = null)
        {
            Clear();


            if (builders != null && builders.Length > 0)
            {
                m_builderCustomList.AddRange(builders);
            }

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
            Dictionary<string, int> refCounts = new Dictionary<string, int>();
            foreach(var kv in m_shareMap)
            {
                foreach(var assetPath in kv.Value)
                {
                    string[] dps = AssetDatabase.GetDependencies(assetPath);
                    foreach (var dp in dps)
                    {
                        if (refCounts.TryGetValue(dp, out var refCount))
                        {
                            refCounts[dp] = refCount + 1;
                        }
                        else
                        {
                            refCounts.Add(dp, 1);
                        }
                    }
                }
            }

            Dictionary<string, List<string>> assetNamesMap = new Dictionary<string, List<string>>();
            foreach(var refKV in refCounts)
            {
                if (refKV.Value > 1)
                {
                    //全部打到Share里去
                    string shareBundleName = AssetBuildConfiger.GetInstance().GetBuildBundleShareName(refKV.Key);
                    if (!string.IsNullOrEmpty(shareBundleName))
                    {
                        List<string> assetList = null;
                        if (!assetNamesMap.TryGetValue(shareBundleName, out assetList))
                        {
                            assetList = new List<string>();
                            assetNamesMap.Add(shareBundleName, assetList);
                        }
                        assetList.Add(refKV.Key);
                    }
                }
            }


            List<AssetBundleBuild> shareBuildList = new List<AssetBundleBuild>();
            foreach(var kv in assetNamesMap)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = kv.Key;
                build.assetNames = kv.Value.ToArray();

                shareBuildList.Add(build);
            }
            m_bundleBuilds.Add("Share", shareBuildList);
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

    }

}
