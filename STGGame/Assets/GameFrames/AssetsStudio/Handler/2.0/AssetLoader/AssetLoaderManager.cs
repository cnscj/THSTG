
using System;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
//全部采用异步
namespace ASGame
{
    public class AssetLoaderManager
    {
        private AssetLoadMode loadMode;
        private BaseLoader m_resourceLoader;
        private BaseLoader m_bundleLoader;

        public int LoadAsset<T>(string path, Action<T> onSuccess = null, Action<int> onFailed = null) where T:Object
        {
            var loader = GetLoader<T>(path);
            var handler = loader.StartLoad(path);
            handler.onCallback += (AssetLoadResult result) =>
            {
                if (result.isDone)
                {
                    onSuccess?.Invoke(result.GetAsset<T>());
                }
                else
                {
                    onFailed?.Invoke(AssetLoadStatus.LOAD_FAILED);
                }
            };
            return handler.id;
        }

        public void BreakLoad(int id)
        {
            //TODO:
        }

        private BaseLoader GetLoader<T>(string path)
        {
            //根据类型判断
            if (typeof(T) == typeof(AssetBundle))
            {
                m_bundleLoader = (m_bundleLoader != null) ? m_bundleLoader : new BundleLoader();
                return m_bundleLoader;
            }
            else
            {
                //第一路径为Assetbundle
                if (path.IndexOf('|') >= 0)
                {
                    m_bundleLoader = (m_bundleLoader != null) ? m_bundleLoader : new BundleLoader();
                    return m_bundleLoader;
                }
                else
                {
#if UNITY_EDITOR
                    m_resourceLoader = (m_resourceLoader != null) ? m_resourceLoader : new ASEditor.EditorLoader();
                    return m_resourceLoader;
#else
                    m_resourceLoader = (m_resourceLoader != null) ? m_resourceLoader : new ResourcesLoader();
                    return m_resourceLoader;
#endif
                }
            }
        }
    }

}
