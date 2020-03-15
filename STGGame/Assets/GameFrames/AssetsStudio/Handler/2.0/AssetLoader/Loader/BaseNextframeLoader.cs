using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IEnumerator = System.Collections.IEnumerator;
namespace ASGame
{
    public abstract class BaseNextframeLoader : BaseLoader
    {
        LinkedList<AssetLoadHandler> m_loadQueue = new LinkedList<AssetLoadHandler>();
        public override AssetLoadHandler StartLoad(string path)
        {
            var handler = GetOrCreateHandler();
            handler.loader = this;
            handler.assetPath = path;

            m_loadQueue.AddLast(handler);
            return handler;
        }

        public override void StopLoad(AssetLoadHandler handler)
        {
            if (handler == null)
                return;

            foreach (var itHandler in m_loadQueue)
            {
                if (itHandler == handler)
                {
                    m_loadQueue.Remove(itHandler);
                    RecycleHandler(itHandler);
                }
            }
        }

        private void Update()
        {
            while (m_loadQueue.Count >0)
            {
                var itHandler = m_loadQueue.First.Value;
                OnLoadAsset(itHandler);
                m_loadQueue.RemoveFirst();

                RecycleHandler(itHandler);
            }
        }

        protected abstract IEnumerator OnLoadAsset(AssetLoadHandler handler);
    }
}

