
using System;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetLoaderManager : MonoSingleton<AssetLoaderManager>
    {
        private BaseLoader m_binaryLoader;
        private BaseLoader m_networkLoader;
        private BaseLoader m_editorLoader;
        private BaseLoader m_resourceLoader;
        private BaseLoader m_bundleLoader;
        public void LoadBundleMainfest(string mainfestPath)
        {
            //XXX:然而大多数情况下下Mainfest是加密的,可能需要先用到二进制打开
            GetOrCreateBundleLoader().LoadMainfest(mainfestPath);
        }

        public int LoadAsset<T>(string path, Action<T> onSuccess = null, Action<int> onFailed = null) where T : Object
        {
            BaseLoader loader;
            string rightPath;

            SelectLoaderAndPath<T>(path,out loader, out rightPath);

            var handler = loader.StartLoad(rightPath);

            handler.OnCompleted((AssetLoadResult result) =>
            {
                if (handler.status == AssetLoadStatus.LOAD_FINISHED)
                {
                    onSuccess?.Invoke(result.GetAsset<T>());
                }
                else
                {
                    onFailed?.Invoke(handler.status);
                }

            });
            return handler.id;
        }

        public void LoadAbort(int id)
        {
            var handler = AssetLoadHandlerManager.GetInstance().GetLoadHandler(id);
            if (handler != null)
            {
                handler.loader?.StopLoad(handler);
            }
        }

        public BinarylLoader GetOrCreateBinaryLoader()
        {
            m_binaryLoader = m_binaryLoader ?? CreateLoader<BinarylLoader>();
            return m_binaryLoader as BinarylLoader;
        }

        public NetworkLoader GetOrCreateNetworkLoader()
        {
            m_networkLoader = m_networkLoader ?? CreateLoader<NetworkLoader>();
            return m_networkLoader as NetworkLoader;
        }

        public BundleLoader GetOrCreateBundleLoader()
        {
            m_bundleLoader = m_bundleLoader ?? CreateLoader<BundleLoader>();
            return m_bundleLoader as BundleLoader;
        }

        public ResourcesLoader GetOrCreateResourceeLoader()
        {
            m_resourceLoader = m_resourceLoader ?? CreateLoader<ResourcesLoader>();
            return m_resourceLoader as ResourcesLoader;
        }

        private EditorLoader GetOrCreateEditorLoader()
        {
            m_editorLoader = m_editorLoader ?? CreateLoader<EditorLoader>();
            return m_editorLoader as EditorLoader;
        }

        private BaseLoader CreateLoader<T>() where T : BaseLoader
        {
            GameObject loaderGO = new GameObject(typeof(T).Name);
            loaderGO.transform.SetParent(transform);
            var loader = loaderGO.AddComponent<T>();
            return loader;
        }

        private void SelectLoaderAndPath<T>(string path, out BaseLoader loader, out string realpath) where T: Object
        {

            //如果是双路径
            if (path.IndexOf('|') >= 0)
            {
#if UNITY_EDITOR    //编辑器下直接加载源路径
                loader = GetOrCreateEditorLoader();
                string[] pathPairs = path.Split('|');
                string assetName = pathPairs[1];
                realpath = assetName;
#else
                loader =  GetOrCreateBundleLoader();
                realpath = path;
#endif
            }
            else
            {
                    //是否是Asset开头的
#if UNITY_EDITOR
                if (path.StartsWith("assets",StringComparison.OrdinalIgnoreCase))
                {
                    loader = GetOrCreateEditorLoader();
                    realpath = path;
                    return;
                }
#endif
                //最后采用ResourceLoader
                loader = GetOrCreateResourceeLoader();
                realpath = path;
            }
            
        }
    }

}
