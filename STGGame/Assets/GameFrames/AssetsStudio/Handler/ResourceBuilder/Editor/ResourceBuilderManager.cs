using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using XLibrary.Package;
using XLibrary;
using System.IO;

namespace ASEditor
{
    public class ResourceBuilderManager : Singleton<ResourceBuilderManager>
    {
        //不同平台下打各自的包
        //公共的抽出来打一个(就是依赖数>1的
        private List<ResourceBuilder> m_builders = new List<ResourceBuilder>();
        private List<ResourceBuilder> m_defaultBuilders = new List<ResourceBuilder>();
        protected BuildAssetBundleOptions m_bundleOptions;

        private string m_finalExportFolder;

        public ResourceBuilderManager(ResourceBuilder[] builders)
        {
            foreach (var builer in builders)
            {
                m_builders.Add(builer);
            }

            //打包设置:
            m_bundleOptions = BuildAssetBundleOptions.None;
            m_bundleOptions |= BuildAssetBundleOptions.ChunkBasedCompression;
            m_bundleOptions |= BuildAssetBundleOptions.DeterministicAssetBundle;
            //听大佬说,使用全路径加载会快一点.....
            m_bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileName;  //没有扩展的加载直接禁用了
            if (ResourceBuilderConfig.GetInstance().bundleIsUseFullPath)
            {
                m_bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;
            }

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
            AssetDatabase.Refresh();//后处理文件

            BuildBefore();
            Build();
            BuildAfter();

            AssetDatabase.Refresh();//打包后刷新，不加这行代码的话要手动刷新才可以看得到打包后的Assetbundle包
            if (ResourceBuilderConfig.GetInstance().isClearAfterBuilded)
            {
                Clear();
            }
        }

        void BuildBefore()
        {
            //拼出输出路径
            m_finalExportFolder = null;

            string exportFolder = ResourceBuilderConfig.GetInstance().exportFolder;
            string finalExportFolder = exportFolder;
            var buildPlatform = ResourceBuilderConfig.GetInstance().GetBuildType();
            if (ResourceBuilderConfig.GetInstance().isUsePlatformName)
            {
                if (ResourceBuilderConfig.GetInstance().targetType == ResourceBuilderConfig.BuildPlatform.Auto)
                {
                    string buildPlatformStr = Enum.GetName(typeof(BuildTarget), buildPlatform);
                    finalExportFolder = XPathTools.Combine(exportFolder, buildPlatformStr).Replace("\\", "/");
                }
                else
                {
                    string buildPlatformStr = Enum.GetName(typeof(ResourceBuilderConfig.BuildPlatform), ResourceBuilderConfig.GetInstance().targetType);
                    finalExportFolder = XPathTools.Combine(exportFolder, buildPlatformStr).Replace("\\", "/");
                }

            }

            if (!XFolderTools.Exists(finalExportFolder))
            {
                XFolderTools.CreateDirectory(finalExportFolder);
            }

            m_finalExportFolder = finalExportFolder;
        }

        void Build()
        {
            if (!string.IsNullOrEmpty(m_finalExportFolder))
            {
                var buildPlatform = ResourceBuilderConfig.GetInstance().GetBuildType();
                BuildPipeline.BuildAssetBundles(m_finalExportFolder, m_bundleOptions, buildPlatform);
                
            }
            else
            {
                Debug.LogWarning("输出目录不能为空");
            }

        }


        void BuildAfter()
        {
            if (!string.IsNullOrEmpty(m_finalExportFolder))
            {
                //移除之前无效的文件夹
                HashSet<string> pathSet = new HashSet<string>();
                var allBundleNames = AssetDatabase.GetAllAssetBundleNames();
                foreach (var bundleName in allBundleNames)
                {
                    pathSet.Add(bundleName);
                    pathSet.Add(string.Format("{0}.manifest", bundleName));
                }
                string exportFolderName = Path.GetFileNameWithoutExtension(m_finalExportFolder).ToLower();
                pathSet.Add(exportFolderName);
                pathSet.Add(string.Format("{0}.manifest", exportFolderName));

                XFolderTools.TraverseFiles(m_finalExportFolder, (fullPath) =>
                {
                    string relaPath = XFileTools.GetFileRelativePath(fullPath).ToLower();
                    string smallFinalPath = m_finalExportFolder.ToLower();
                    string bundlePath = relaPath.Replace(string.Format("{0}/", smallFinalPath), "");

                    if (!pathSet.Contains(bundlePath))
                    {
                        // 无效的文件
                        XFileTools.Delete(fullPath);
                        XFileTools.Delete(string.Format("{0}.manifest", fullPath));
                    }
                }, true);

                XFolderTools.TraverseFolder(m_finalExportFolder, (folderPath) =>
                {
                    if (XFolderTools.CheckNullFolder(folderPath))
                    {
                        XFolderTools.DeleteDirectory(folderPath);
                    }
                }, true);
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

