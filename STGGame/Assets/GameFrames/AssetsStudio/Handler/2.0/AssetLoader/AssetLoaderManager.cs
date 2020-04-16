﻿
using System;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetLoaderManager : MonoSingleton<AssetLoaderManager>
    {
        private BaseLoader networkLoader;
        private BaseLoader m_editorLoader;
        private BaseLoader m_resourceLoader;
        private BaseLoader m_bundleLoader;
        public void LoadBundleMainfest(string mainfestPath)
        {
            GetOrCreateBundleLoader().LoadMainfest(mainfestPath);
        }

        public int LoadAsset<T>(string path, Action<T> onSuccess = null, Action<int> onFailed = null) where T : Object
        {
            var loader = GetOrCreateLoader(path,typeof(T));
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

        public void LoadBreak(int id)
        {
            var handler = AssetLoadHandlerManager.GetInstance().GetLoadHandler(id);
            if (handler != null)
            {
                handler.loader?.StopLoad(handler);
            }
        }

        public BundleLoader GetOrCreateBundleLoader()
        {
            m_bundleLoader = m_bundleLoader ?? CreateLoader<BundleLoader>();
            return m_bundleLoader as BundleLoader;
        }

        public BundleLoader GetOrCreateEditorLoader()
        {
            m_editorLoader = m_editorLoader ?? CreateLoader<EditorLoader>();
            return m_editorLoader as BundleLoader;
        }

        public BundleLoader GetOrCreateResourceeLoader()
        {
            m_resourceLoader = m_resourceLoader ?? CreateLoader<ResourcesLoader>();
            return m_resourceLoader as BundleLoader;
        }

        private BaseLoader CreateLoader<T>() where T : BaseLoader
        {
            GameObject loaderGO = new GameObject(typeof(T).Name);
            loaderGO.transform.SetParent(transform);
            var loader = loaderGO.AddComponent<T>();
            return loader;
        }

        private BaseLoader GetOrCreateLoader(string path, Type type)
        {
            if (type == typeof(AssetBundle))
            {
                return GetOrCreateBundleLoader();
            }
            else
            {
                //如果是双路径,则为
                if (path.IndexOf('|') >= 0)
                {
                    return GetOrCreateBundleLoader();
                }
                else
                {
                    //是否是Asset开头的
#if UNITY_EDITOR
                    if (path.StartsWith("assets",StringComparison.OrdinalIgnoreCase))
                    {
                        return GetOrCreateEditorLoader();
                    }
#endif
                    //最后采用ResourceLoader
                    return GetOrCreateResourceeLoader();
                }
            }
        }
    }

}
