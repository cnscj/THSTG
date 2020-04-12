using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace ASEditor
{
    public class AssetCommonBuilder : AssetBaseBuilder
    {
        protected AssetCommonBuildItem buildItem;
        public AssetCommonBuilder(AssetCommonBuildItem item) : base(item.builderName)
        {
            buildItem = item;
        }

        protected override string[] OnFiles()
        {
            string buildSuffix = buildItem.buildSuffix ?? "*.*";
            string[] files = Directory.GetFiles(buildItem.buildSrcPath, buildSuffix, SearchOption.AllDirectories)
                   .ToArray();

            return files;
        }

        protected override AssetBundlePair[] OnBundles(string assetPath)
        {
            List<AssetBundlePair> bundleList = new List<AssetBundlePair>();

            //处理资源
            var assetBundleName = GetCommonBundleName(assetPath);
            bundleList.Add(new AssetBundlePair(assetPath, assetBundleName));

            //依赖引用处理
            var depFiles = GetDependencies(assetPath);
            if (depFiles != null && depFiles.Length > 0)
            {
                foreach (var depFile in depFiles)
                {
                    if (string.Compare(depFile, assetPath) == 0)
                        continue;

                    var refFiles = GetReferenceds(depFile);
                    if (refFiles != null)
                    {
                        if (refFiles.Length == 1)
                        {
                            //处理单一引用
                            string firstRefAssetPath = refFiles[0];
                            if (string.Compare(firstRefAssetPath, assetPath) == 0)
                            {
                                var commonBundleName = GetCommonBundleName(firstRefAssetPath);
                                bundleList.Add(new AssetBundlePair(firstRefAssetPath, commonBundleName));
                            }
                        }
                        else
                        {
                            //处理公共的
                            var shareBundleName = GetShareBundleName(depFile);
                            bundleList.Add(new AssetBundlePair(depFile, shareBundleName));
                        } 
                    }
                }
            }
            return bundleList.ToArray();
        }

        private string GetShareBundleName(string assetPath)
        {
            if (string.IsNullOrEmpty(buildItem.shareBundleNameFormat))
            {
                string assetBundleName = AssetBuildConfiger.GetInstance().GetBuildBundleShareName(assetPath);
                return assetBundleName;
            }
            else
            {
                string buildName = GetName();

                string nameFormat = buildItem.shareBundleNameFormat;
                nameFormat = AssetBuildConfiger.GetInstance().GetFormatBundleName(nameFormat, assetPath);
                nameFormat = nameFormat.Replace("{buildName}", buildName);

                nameFormat = nameFormat.ToLower();
                return nameFormat;
            }
        }

        private string GetCommonBundleName(string assetPath)
        {
            if (string.IsNullOrEmpty(buildItem.assetBundleNameFormat))
            {
                string assetFileName = Path.GetFileName(assetPath);
                string assetBundleName = AssetBuildConfiger.GetInstance().GetBuildBundleName(_builderName, assetFileName, buildItem.commonPrefixBuildOne);
                return assetBundleName;
            }
            else
            {
                string buildName = GetName();

                string nameFormat = buildItem.assetBundleNameFormat;
                nameFormat = AssetBuildConfiger.GetInstance().GetFormatBundleName(nameFormat, assetPath);
                nameFormat = nameFormat.Replace("{buildName}", buildName);

                nameFormat = nameFormat.ToLower();
                return nameFormat;
            }
        }
    }
}
