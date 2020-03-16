
using System;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetLoaderManager : Singleton<AssetLoaderManager>
    {
        private BaseLoader m_editorOrResLoader;
        private BaseLoader m_bundleLoader;

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

        private BaseLoader GetOrCreateLoader<T>(string path)
        {
            //根据类型判断
            if (typeof(T) == typeof(AssetBundle))
            {
                m_bundleLoader = m_bundleLoader ?? new BundleLoader();
                return m_bundleLoader;
            }
            else
            {
                //如果是复合路径,则使用Bundle加载器
                if (path.IndexOf('|') >= 0)
                {
                    m_bundleLoader = m_bundleLoader ?? new BundleLoader();
                    return m_bundleLoader;
                }
                else
                {
#if UNITY_EDITOR
                    m_editorOrResLoader = m_editorOrResLoader ?? new ASEditor.EditorLoader();
                    return m_editorOrResLoader;
#else
                    m_editorOrResLoader = m_editorOrResLoader ?? new ResourcesLoader();
                    return m_editorOrResLoader;
#endif
                }
            }
        }
    }

}
