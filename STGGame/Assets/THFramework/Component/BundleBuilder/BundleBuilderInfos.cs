
namespace THEditor
{
    [System.Serializable]
    public class BundleBuilderInfos
    {
        public string srcName = "";
        public string srcResFolder = "";
        public string exportFolder = "";

        private string m_bundleLabel = "";
        public string bundleLabel
        {
            get
            {
                return m_bundleLabel;
            }
            set
            {
                m_bundleLabel = value.ToLower();
            }
        }
    }

}
