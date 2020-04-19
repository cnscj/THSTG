using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IEnumerator = System.Collections.IEnumerator;
namespace ASGame
{
    public abstract class BaseNextframeLoader : BaseLoader
    {
        private LinkedList<KeyValuePair<int, AssetLoadHandler>> m_readyQueue = new LinkedList<KeyValuePair<int, AssetLoadHandler>>();       //延迟队列

        protected override void OnStartLoad(AssetLoadHandler handler)
        {
            handler.loader = this;
            m_readyQueue.AddLast(new KeyValuePair<int, AssetLoadHandler>(handler.id, handler));
        }

        protected override void OnStopLoad(AssetLoadHandler handler)
        {
            if (handler == null)
                return;

            for (LinkedListNode<KeyValuePair<int, AssetLoadHandler>> iterNode = m_readyQueue.Last; iterNode != null; iterNode = iterNode.Previous)
            {
                var itPair = iterNode.Value;
                if (itPair.Key == handler.id)
                {
                    m_readyQueue.Remove(itPair);
                    break;
                }
            }
        }

        public override void Clear()
        {
            m_readyQueue.Clear();
            base.Clear();
        }

        protected override void OnUpdate()
        {
            UpdateReady();
        }

        private void UpdateReady()
        {
            while (m_readyQueue.Count > 0)
            {
                var itPair = m_readyQueue.First.Value;
                OnLoadAsset(itPair.Value);
                m_readyQueue.RemoveFirst();
            }
        }

        protected abstract IEnumerator OnLoadAsset(AssetLoadHandler handler);
    }
}

