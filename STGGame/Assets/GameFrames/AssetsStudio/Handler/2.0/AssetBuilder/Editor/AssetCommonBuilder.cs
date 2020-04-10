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

        //TODO:公共资源处理
        protected override void OnBefore(string[] files)
        {
            //这里剔除两部分,公共的和单独的
            if (files == null || files.Length <= 0)
                return;

            foreach(var assetPath in files)
            {
                var depFiles = AssetDatabase.GetDependencies(assetPath);
                foreach(var depFile in depFiles)
                {
                    if (string.Compare(assetPath, depFile) == 0)
                        continue;


                }
            }
        }

        protected override string OnName(string assetPath)
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
