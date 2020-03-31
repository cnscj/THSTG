using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASEditor
{
    [System.Serializable]
    public class AssetCommonBuildItem
    {
        public string builderName;
        public string buildSrcPath;
        public string buildSuffix = "*.*";
        public bool commonPrefixBuildOne = true;


    }
}
