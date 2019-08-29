using UnityEngine;
using System.Collections;
using THGame.Package;
using System.Collections.Generic;
using UnityEditor;
using THGame;
using System;

namespace THEditor
{
    public class ResourceBuilderManager : Singleton<ResourceBuilderManager>
    {
        //不同平台下打各自的包
        //公共的抽出来打一个(就是依赖数>1的
        private List<ResourceBuilder> m_builders = new List<ResourceBuilder>();
        private List<ResourceBuilder> m_defaultBuilders = new List<ResourceBuilder>();
        protected BuildAssetBundleOptions m_bundleOptions;

        public ResourceBuilderManager(ResourceBuilder [] builders)
        {
            foreach (var builer in builders)
            {
                m_builders.Add(builer);
            }

            //打包设置:
            m_bundleOptions = BuildAssetBundleOptions.None;
            m_bundleOptions |= BuildAssetBundleOptions.ChunkBasedCompression;
            //听大佬说,使用全路径加载会快一点.....
            m_bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileName;
            //m_bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;
        }
        public void BuildAll()
        {
            Clear();
            //
            foreach (var info in ResourceBuilderConfig.GetInstance().buildInfoList)
            {
                string srcName = info.srcName.ToLower();
                m_defaultBuilders.Add(new ResourceBuilderSimple(info)); 
            }

            foreach (var builer in m_builders)
            {
                builer.Build();
            }

            foreach (var builer in m_defaultBuilders)
            {
                builer.Build();
            }

            //
            Build();

            if (ResourceBuilderConfig.GetInstance().isClearAfterBuilded)
            {
                Clear();
            }
        }

        void Build()
        {
            string exportFolder = ResourceBuilderConfig.GetInstance().exportFolder;
            if (exportFolder != "")
            {
                string finalExportFolder = exportFolder;
                var buildPlatform = ResourceBuilderConfig.GetInstance().GetBuildType();
                if (ResourceBuilderConfig.GetInstance().isUsePlatformName)
                {
                    if (ResourceBuilderConfig.GetInstance().targetType == ResourceBuilderConfig.BuildPlatform.Auto)
                    {
                        string buildPlatformStr = Enum.GetName(typeof(BuildTarget), buildPlatform);
                        finalExportFolder = PathUtil.Combine(exportFolder, buildPlatformStr).Replace("\\", "/");
                    }else
                    {
                        string buildPlatformStr = Enum.GetName(typeof(ResourceBuilderConfig.BuildPlatform), ResourceBuilderConfig.GetInstance().targetType);
                        finalExportFolder = PathUtil.Combine(exportFolder, buildPlatformStr).Replace("\\", "/");
                    }
                    
                }

                if (!XFolderTools.Exists(finalExportFolder))
                {
                    XFolderTools.CreateDirectory(finalExportFolder);
                }
                BuildPipeline.BuildAssetBundles(finalExportFolder, m_bundleOptions, buildPlatform);
                AssetDatabase.Refresh();//打包后刷新，不加这行代码的话要手动刷新才可以看得到打包后的Assetbundle包
            }
            else
            {
                Debug.LogWarning("输出目录不能为空");
            }
            
        }
        void Clear()
        {
            m_defaultBuilders.Clear();

            int length = AssetDatabase.GetAllAssetBundleNames().Length;
            string[] oldAssetBundleNames = new string[length];
            for (int i = 0; i < length; i++)
            {
                oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
            }

            for (int j = 0; j < oldAssetBundleNames.Length; j++)
            {
                AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
            }
        }
    }
}

