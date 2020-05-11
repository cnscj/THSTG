using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASEditor
{
    public class AssetBundlePair
    {
        public readonly string assetPath;
        public readonly string bundleName;

        public AssetBundlePair(string path, string name)
        {
            assetPath = path;
            bundleName = name;
        }

        public bool isEmpty()
        {
            return string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(bundleName);
        }
    }
}

