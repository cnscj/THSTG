using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IEnumerator = System.Collections.IEnumerator;
namespace ASGame
{
    public abstract class BaseCoroutineLoader : BaseLoader
    {
        private Dictionary<int, Coroutine> m_loadCoroutines = new Dictionary<int, Coroutine>();

        public override AssetLoadHandler StartLoad(string path)
        {
            var handler = GetOrCreateHandler();
            handler.loader = this;
            handler.assetPath = path;

            var coroutine = StartCoroutine(LoadAsset(handler));
            m_loadCoroutines[handler.id] = coroutine;

            return handler;
        }

        public override void StopLoad(AssetLoadHandler handler)
        {
            if (handler == null)
                return;

            if (m_loadCoroutines.ContainsKey(handler.id))
            {
                var coroutine = m_loadCoroutines[handler.id];
                StopCoroutine(coroutine);
                RecycleHandler(handler);
            }
        }

        private IEnumerator LoadAsset(AssetLoadHandler handler)
        {
            yield return OnLoadAsset(handler);
            if (!m_loadCoroutines.ContainsKey(handler.id))
            {
                m_loadCoroutines.Remove(handler.id);
            }
            RecycleHandler(handler);
        }

        protected abstract IEnumerator OnLoadAsset(AssetLoadHandler handler);
    }
}

