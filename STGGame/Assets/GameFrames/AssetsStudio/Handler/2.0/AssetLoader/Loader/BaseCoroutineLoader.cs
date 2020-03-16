using System;
using System.Collections.Generic;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;
namespace ASGame
{
    public abstract class BaseCoroutineLoader : BaseLoader
    {
        private Dictionary<int, KeyValuePair<AssetLoadHandler, Coroutine>> m_loadCoroutines = new Dictionary<int, KeyValuePair<AssetLoadHandler, Coroutine>>();

        public override AssetLoadHandler StartLoad(string path)
        {
            var handler = AssetLoadHandlerManager.GetInstance().GetOrCreateHandler();
            handler.loader = this;
            handler.assetPath = path;

            var coroutine = StartCoroutine(LoadAsset(handler));
            m_loadCoroutines[handler.id] = new KeyValuePair<AssetLoadHandler, Coroutine>(handler, coroutine);

            return handler;
        }

        public override void StopLoad(AssetLoadHandler handler)
        {
            if (handler == null)
                return;

            if (m_loadCoroutines.ContainsKey(handler.id))
            {
                var itPair = m_loadCoroutines[handler.id];
                StopCoroutine(itPair.Value);
                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Key);
                m_loadCoroutines.Remove(handler.id);
            }
        }

        public override void Clear()
        {
            foreach(var mapPair in m_loadCoroutines)
            {
                var itPair = mapPair.Value;
                StopCoroutine(itPair.Value);
                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Key);
            }
            m_loadCoroutines.Clear();
        }

        private IEnumerator LoadAsset(AssetLoadHandler handler)
        {
            yield return OnLoadAsset(handler);
            if (!m_loadCoroutines.ContainsKey(handler.id))
            {
                m_loadCoroutines.Remove(handler.id);
            }
            AssetLoadHandlerManager.GetInstance().RecycleHandler(handler);
        }

        protected abstract IEnumerator OnLoadAsset(AssetLoadHandler handler);
    }
}

