using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASEditor
{
    [System.Serializable]
    public class AssetCommonBuildItem
    {
        public string srcName = "";
        public string srcResFolder = "";
        public string srcBundleSuffix = "";
        public bool isTraversal = true;

        public string bundleName = "{0}/{1}/{2}.ab";
        public bool isUsePathName = false;
        public bool isCommonPrefixion = false;

        public string shareBundleName = "share/{0}.ab";
    }
}
