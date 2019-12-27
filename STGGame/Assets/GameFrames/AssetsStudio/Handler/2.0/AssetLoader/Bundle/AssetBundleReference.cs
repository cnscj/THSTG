
using UnityEngine;
using XLibGame;

namespace ASGame
{
    public class AssetBundleReference : Reference
    {
        private AssetBundle m_assetBundle;

        public AssetBundleReference(AssetBundle bundle)
        {
            m_assetBundle = bundle;
        }

        public T Load<T>(string name) where T : class
        {
            if (m_assetBundle != null)
            {
                return m_assetBundle.LoadAsset(name,typeof(T)) as T;
            }
            return default;
        }

        public AssetBundle GetBundle()
        {
            return m_assetBundle;
        }

        protected override void OnRelease()
        {
            if (m_assetBundle != null)
            {
                m_assetBundle.Unload(true);
                m_assetBundle = null;
            }
        }

        //
        private void LoadCompleted()
        {

        }
    }
}