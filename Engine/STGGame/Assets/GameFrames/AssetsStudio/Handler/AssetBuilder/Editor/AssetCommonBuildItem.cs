﻿namespace ASEditor
{
    [System.Serializable]
    public class AssetCommonBuildItem
    {
        public bool isEnabled = true;
        public string builderName = "";
        public string buildSrcPath = "";
        public string regexFileName = ".*";
        public string assetBundleNameFormat = "{buildName}/{assetNameNotEx}.ab";
        public string shareBundleNameFormat = "";
        public bool commonPrefixBuildOne = false;        //相同前缀打成一个


    }
}
