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
        protected override void OnStartLoad(AssetLoadHandler handler)
        {
            handler.loader = this;

            m_loadQueue.AddLast(new KeyValuePair<int, AssetLoadHandler>(handler.id, handler));
        }

        protected override void OnStopLoad(AssetLoadHandler handler)
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

        protected override void OnClear()
        {
            foreach(var itPair in m_loadQueue)
            {
                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
            }
            m_loadQueue.Clear();
        }

        protected override void OnUpdate()
        {
            while (m_loadQueue.Count > 0)
            {
                var itPair = m_loadQueue.First.Value;
                OnLoadAsset(itPair.Value);
                m_loadQueue.RemoveFirst();

                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
            }
        }

        protected override int OnLoadingCount()
        {
            return m_loadQueue.Count;
        }

        protected abstract IEnumerator OnLoadAsset(AssetLoadHandler handler);
    }
}

