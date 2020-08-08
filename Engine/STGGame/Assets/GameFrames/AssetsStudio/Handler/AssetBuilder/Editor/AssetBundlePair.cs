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

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(bundleName);
        }
    }
}

