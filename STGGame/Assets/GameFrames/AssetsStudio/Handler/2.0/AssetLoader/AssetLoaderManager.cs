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
        private BaseLoader m_editorOrResLoader;
        private BaseLoader m_bundleLoader;
        public bool LoadBundleMainfest(string mainfestPath)
        {
            return GetOrCreateBundleLoader().LoadMainfest(mainfestPath);
        }

        public int LoadAsset<T>(string path, Action<T> onSuccess = null, Action<int> onFailed = null) where T : Object
        {
            var loader = GetOrCreateLoader<T>(path);
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
            var handler = AssetLoadHandlerManager.GetInstance().GetLoadHandler(id);
            if (handler != null)
            {
                handler.loader?.StopLoad(handler);
            }
        }

        private BundleLoader GetOrCreateBundleLoader()
        {
            m_bundleLoader = m_bundleLoader ?? CreateLoader<BundleLoader>();
            return m_bundleLoader as BundleLoader;
        }

        private BaseLoader GetOrCreateLoader<T>(string path)
        {
            //根据类型判断
            if (typeof(T) == typeof(AssetBundle))
            {
                return GetOrCreateBundleLoader();
            }
            else
            {
                //如果是复合路径,则使用Bundle加载器
                if (path.IndexOf('|') >= 0)
                {
                    return GetOrCreateBundleLoader();
                }
                else
                {
#if UNITY_EDITOR
                    m_editorOrResLoader = m_editorOrResLoader ?? CreateLoader<ASEditor.EditorLoader>();
                    return m_editorOrResLoader;
#else
                    m_editorOrResLoader = m_editorOrResLoader ?? CreateLoader<ResourcesLoader>();
                    return m_editorOrResLoader;
#endif
                }
            }
        }

        private BaseLoader CreateLoader<T>() where T : BaseLoader
        {
            GameObject loaderGO = new GameObject(typeof(T).Name);
            loaderGO.transform.SetParent(transform);
            var loader = loaderGO.AddComponent<T>();
            return loader;
        }
    }

}
