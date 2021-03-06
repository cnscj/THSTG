﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XLibrary;
using XLibrary.Package;

namespace ASEditor
{
    public class AssetBuilderManager : Singleton<AssetBuilderManager>
    {
        public class BuilderComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                int iResult = (int)x - (int)y;
                if (iResult == 0) iResult = -1;
                return iResult;
            }
        }
        public static readonly string SHARE_BUILDER_NAME = "_GlobalShare_";
        //需要进行一次全局依赖Share打包
        private List<AssetBaseBuilder> m_builderCommonList = new List<AssetBaseBuilder>();
        private List<AssetBaseBuilder> m_builderCustomList = new List<AssetBaseBuilder>();

        private Dictionary<string, List<string>> m_buildAssetMap = new Dictionary<string, List<string>>();
        private Dictionary<string, List<AssetBundlePair>> m_bundleBuilds = new Dictionary<string, List<AssetBundlePair>>();

        public void Build(AssetBaseBuilder []builders = null)
        {
            Clear();

            if (builders != null && builders.Length > 0)
            {
                m_builderCustomList.AddRange(builders);
            }
            Build();


            AssetDatabase.Refresh();
        }

        public void Clear()
        {
            m_builderCommonList.Clear();
            m_builderCustomList.Clear();

            m_buildAssetMap.Clear();
            m_bundleBuilds.Clear();
        }

        private void Build()
        {
            if (AssetBuildConfiger.GetInstance().isUseDependenciesCache) AssetBuildRelationship.TryLoadCache();
            BuildBefore();
            BuildAll();
            BuildAfter();
            if (AssetBuildConfiger.GetInstance().isUseDependenciesCache) AssetBuildRelationship.SaveCache();
        }

        private void BuildBefore()
        {
            //
            var commonItems = AssetBuildConfiger.GetInstance().buildItemList;
            if (commonItems != null && commonItems.Count > 0)
            {
                foreach (var item in commonItems)
                {
                    if (!item.isEnabled)
                        continue;

                    if (string.IsNullOrEmpty(item.builderName))
                        continue;

                    AssetBaseBuilder builder = new AssetCommonBuilder(item);
                    m_builderCommonList.Add(builder);
                }
            }

            //通过反射获取所有类
            Type[] types = Assembly.GetCallingAssembly().GetTypes();
            List<Type> builderClassList = new List<Type>();
            Type customBuilderType = typeof(AssetCustomBuilder);
            foreach (Type item in types)
            {
                if (item.IsAbstract) continue;
                if (item == customBuilderType) continue;

                if (item.BaseType == customBuilderType)
                {
                    builderClassList.Add(item);
                }
            }

            SortedList<int, AssetBaseBuilder> sortedClassList = new SortedList<int, AssetBaseBuilder>(new BuilderComparer());
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
                builder.Clear();
                var fileList = builder.GetFileList();
                m_buildAssetMap.Add(builder.GetName(), fileList);

                var bundleList = builder.GetBundleList();
                m_bundleBuilds.Add(builder.GetName(), bundleList);
            }
        }

        private void Build4Custom()
        {
            foreach (var builder in m_builderCustomList)
            {
                builder.Clear();
                var fileList = builder.GetFileList();
                m_buildAssetMap.Add(builder.GetName(), fileList);

                var bundleList = builder.GetBundleList();
                m_bundleBuilds.Add(builder.GetName(), bundleList);
            }
        }

        private List<string> CollectShare()
        {
            Dictionary<string, int> refCounts = new Dictionary<string, int>();
            foreach(var kv in m_buildAssetMap)
            {
                foreach(var fullPath in kv.Value)
                {
                    string assetPath = XPathTools.GetRelativePath(fullPath);
                    string[] dps = AssetBuildRelationship.GetDependencies(assetPath);
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

            List<string> shareAssetList = new List<string>();
            foreach(var refKV in refCounts)
            {
                if (refKV.Value > 1)
                {
                    shareAssetList.Add(refKV.Key); 
                }
            }

            return shareAssetList;
        }

        //这里需要做下合并,减少ab数量:一个依赖的与依赖合并，一个引用的与引用合并
        private void Build4Share()
        {
            List<string> shareAssetList = CollectShare();
            List<AssetBundlePair> shareBundleList = GetOrCreateShareList();

            Dictionary<string, string> depMap = new Dictionary<string, string>();
            Dictionary<string, string> refMap = new Dictionary<string, string>();
            if (AssetBuildConfiger.GetInstance().isOptimzeShareBundle)
            {
                foreach (var assetPath in shareAssetList)
                {
                    string[] dps = AssetBuildRelationship.GetDependencies(assetPath);
                    foreach (var dp in dps)
                    {
                        //忽略自己
                        if (string.Compare(assetPath,dp,true) == 0)
                            continue;

                        // 单一依赖
                        if (dps.Length == 2)
                        {
                            depMap[assetPath] = dp;
                        }

                        //单一引用
                        if (refMap.ContainsKey(dp))
                        {
                            refMap[dp] = "";    //已经不是单一引用了
                        }
                        else
                        {
                            refMap[dp] = assetPath;
                        }
                    }
                }
            }

            foreach (var assetPath in shareAssetList)
            {
                string shareBundleName = AssetBuildConfiger.GetInstance().GetBuildBundleShareName(assetPath);//全部打到Share里去
                if (depMap.TryGetValue(assetPath,out var depFilePath))
                {
                    shareBundleName = AssetBuildConfiger.GetInstance().GetBuildBundleShareName(depFilePath);
                }
                else if(refMap.TryGetValue(assetPath,out var refFilePath))
                {
                    if(!string.IsNullOrEmpty(refFilePath))
                    {
                        shareBundleName = AssetBuildConfiger.GetInstance().GetBuildBundleShareName(refFilePath);
                    }
                }

                shareBundleList.Add(new AssetBundlePair(assetPath, shareBundleName));
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

            if (!XFolderTools.Exists(buildExportPath))
                XFolderTools.CreateDirectory(buildExportPath);

            BuildPipeline.BuildAssetBundles(buildExportPath, GetBuildsArray(), bundleOptions, buildPlatform);
        }

        private void BuildAfter()
        {
            AssetDatabase.Refresh();
        }

        private AssetBundleBuild[] GetBuildsArray()
        {
            List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();
            foreach (var bundleList in m_bundleBuilds.Values)
            {
                if (bundleList == null || bundleList.Count <= 0)
                    continue;

                HashSet<string> fileSet = new HashSet<string>();
                Dictionary<string, HashSet<string>> buildMap = new Dictionary<string, HashSet<string>>();
                foreach (var pair in bundleList)
                {
                    if (pair.IsEmpty())
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
                foreach (var kv in buildMap)
                {
                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = kv.Key;
                    build.assetNames = kv.Value.ToArray();

                    buildList.Add(build);
                }
            }
            return buildList.ToArray();
        }

        private List<AssetBundlePair> GetOrCreateShareList()
        {
            List<AssetBundlePair> shareBundleList = null;
            if (!m_bundleBuilds.TryGetValue(SHARE_BUILDER_NAME, out shareBundleList))
            {
                shareBundleList = new List<AssetBundlePair>();
                m_bundleBuilds.Add(SHARE_BUILDER_NAME, shareBundleList);
            }
            return shareBundleList;
        }
    }


}
