﻿using System.Collections;
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
        protected override string OnName(string assetPath)
        {
            string assetFileName = Path.GetFileName(assetPath);
            string assetBundleName = AssetBuildConfiger.GetInstance().GetBuildBundleName(_builderName, assetFileName, buildItem.commonPrefixBuildOne);
            return assetBundleName;
        }

    }
}