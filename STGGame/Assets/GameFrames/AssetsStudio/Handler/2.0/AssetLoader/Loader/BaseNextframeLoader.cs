using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IEnumerator = System.Collections.IEnumerator;
namespace ASGame
{
    public abstract class BaseNextframeLoader : BaseLoader
    {
        LinkedList<KeyValuePair<int, AssetLoadHandler>> m_loadQueue = new LinkedList<KeyValuePair<int, AssetLoadHandler>>();
        public override AssetLoadHandler StartLoad(string path)
        {
            var handler = AssetLoadHandlerManager.GetInstance().GetOrCreateHandler();
            handler.loader = this;
            handler.assetPath = path;

            m_loadQueue.AddLast(new KeyValuePair<int, AssetLoadHandler>(handler.id, handler));
            return handler;
        }

        public override void StopLoad(AssetLoadHandler handler)
        {
            if (handler == null)
                return;

            foreach (var itPair in m_loadQueue)
            {
                if (itPair.Key == handler.id)
                {
                    m_loadQueue.Remove(itPair);
                    AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
                }
            }
        }

        public override void Clear()
        {
            foreach(var itPair in m_loadQueue)
            {
                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
            }
            m_loadQueue.Clear();
        }

        private void Update()
        {
            while (m_loadQueue.Count > 0)
            {
                var itPair = m_loadQueue.First.Value;
                OnLoadAsset(itPair.Value);
                m_loadQueue.RemoveFirst();

                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
            }
        }

        protected abstract IEnumerator OnLoadAsset(AssetLoadHandler handler);
    }
}

