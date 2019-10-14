
namespace ASGame
{
    [System.Serializable]
    public class ResourceBuilderInfos
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
