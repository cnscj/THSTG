using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using XLibrary;
using XLibrary.Package;

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
                   //.Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower()))
                   .ToArray();

            return files;
        }
        protected override string OnBundleName(string assetPath)
        {
            if (string.IsNullOrEmpty(buildItem.bundleNameFormat))
            {
                string assetFileName = Path.GetFileName(assetPath);
                string assetBundleName = AssetBuildConfiger.GetInstance().GetBuildBundleName(_builderName, assetFileName, buildItem.commonPrefixBuildOne);
                return assetBundleName;
            }
            else
            {
                string assetRootPath = Path.GetDirectoryName(assetPath);
                string assetName = Path.GetFileName(assetPath);
                string assetNameNotEx = Path.GetFileNameWithoutExtension(assetPath);
                string assetKey = XStringTools.SplitPathKey(assetPath);
                string buildName = GetName();

                string nameFormat = buildItem.bundleNameFormat;
                nameFormat = nameFormat.Replace("{assetRootPath}", assetRootPath);
                nameFormat = nameFormat.Replace("{assetNameNotEx}", assetNameNotEx);
                nameFormat = nameFormat.Replace("{assetName}", assetName);
                nameFormat = nameFormat.Replace("{assetKey}", assetKey);
                nameFormat = nameFormat.Replace("{buildName}", buildName);

                nameFormat = nameFormat.ToLower();
                return nameFormat;
            }

        }

    }
}
