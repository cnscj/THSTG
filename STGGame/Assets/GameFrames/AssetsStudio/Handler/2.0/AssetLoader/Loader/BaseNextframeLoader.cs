using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IEnumerator = System.Collections.IEnumerator;
namespace ASGame
{
    public abstract class BaseNextframeLoader : BaseLoader
    {
        LinkedList<KeyValuePair<int, AssetLoadHandler>> m_readyQueue = new LinkedList<KeyValuePair<int, AssetLoadHandler>>();      //准备队列
        LinkedList<KeyValuePair<int, AssetLoadHandler>> m_loadQueue = new LinkedList<KeyValuePair<int, AssetLoadHandler>>();        //加载队列

        protected override int OnLoadingCount()
        {
            return m_readyQueue.Count + m_loadQueue.Count;
        }

        protected override void OnStartLoad(AssetLoadHandler handler)
        {
            handler.loader = this;
            handler.status = AssetLoadStatus.LOAD_PREPARE;
            m_readyQueue.AddLast(new KeyValuePair<int, AssetLoadHandler>(handler.id, handler));
        }

        protected override void OnStopLoad(AssetLoadHandler handler)
        {
            if (handler == null)
                return;

            RemoveHandler(handler);
        }

        protected override void OnClear()
        {
            ClearHandlers();
        }

        protected override void OnUpdate()
        {
            UpdateReady();
            UpdateLoading();
        }

        private void UpdateReady()
        {
            while (m_readyQueue.Count > 0)
            {
                var itPair = m_readyQueue.First.Value;

                itPair.Value.status = AssetLoadStatus.LOAD_LOADING;
                OnLoadAsset(itPair.Value);

                m_readyQueue.RemoveFirst();
                m_loadQueue.AddLast(itPair);

                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
            }
        }

        private void UpdateLoading()
        {
            foreach (var handlerPair in m_loadQueue)
            {
                if (IsCompleted(handlerPair.Value))
                {
                    m_loadQueue.Remove(handlerPair);
                }
            }
        }
        private void ClearHandlers()
        {
            foreach (var itPair in m_readyQueue)
            {
                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
            }
            m_readyQueue.Clear();

            foreach (var itPair in m_loadQueue)
            {
                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
            }
            m_loadQueue.Clear();
        }

        private void RemoveHandler(AssetLoadHandler handler)
        {
            for (LinkedListNode<KeyValuePair<int, AssetLoadHandler>> iterNode = m_readyQueue.Last; iterNode != null; iterNode = iterNode.Previous)
            {
                var itPair = iterNode.Value;
                if (itPair.Key == handler.id)
                {
                    m_readyQueue.Remove(itPair);
                    AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
                    break;
                }
            }

            for (LinkedListNode<KeyValuePair<int, AssetLoadHandler>> iterNode = m_loadQueue.Last; iterNode != null; iterNode = iterNode.Previous)
            {
                var itPair = iterNode.Value;
                if (itPair.Key == handler.id)
                {
                    m_loadQueue.Remove(itPair);
                    AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
                    break;
                }
            }
        }

        protected virtual bool IsCompleted(AssetLoadHandler handler){ return true; }
        protected abstract IEnumerator OnLoadAsset(AssetLoadHandler handler);
    }
}

