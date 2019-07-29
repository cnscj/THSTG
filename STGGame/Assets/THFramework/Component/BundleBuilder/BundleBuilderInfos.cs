
namespace THGame
{
    [System.Serializable]
    public class BundleBuilderInfos
    {
        public string srcName = "";
        public string srcResFolder = "";

        private string m_bundleName = "";
        public string bundleName
        {
            get
            {
                return m_bundleName;
            }
            set
            {
                m_bundleName = value.ToLower();
            }
        }
    }

}
