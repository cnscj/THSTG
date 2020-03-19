using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IEnumerator = System.Collections.IEnumerator;
namespace ASGame
{
    public abstract class BaseNextframeLoader : BaseLoader
    {
        LinkedList<KeyValuePair<int, AssetLoadHandler>> m_prepareQueue = new LinkedList<KeyValuePair<int, AssetLoadHandler>>();      //准备队列
        LinkedList<KeyValuePair<int, AssetLoadHandler>> m_loadQueue = new LinkedList<KeyValuePair<int, AssetLoadHandler>>();    //加载队列

        protected override void OnStartLoad(AssetLoadHandler handler)
        {
            handler.loader = this;
            handler.status = AssetLoadStatus.LOAD_PREPARE;
            m_prepareQueue.AddLast(new KeyValuePair<int, AssetLoadHandler>(handler.id, handler));
        }

        protected override void OnStopLoad(AssetLoadHandler handler)
        {
            if (handler == null)
                return;

            RemoveHandler(handler);
        }

        protected override void OnClear()
        {
            foreach (var itPair in m_prepareQueue)
            {
                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
            }
            m_prepareQueue.Clear();

            foreach (var itPair in m_loadQueue)
            {
                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
            }
            m_loadQueue.Clear();
        }

        protected override void OnUpdate()
        {
            UpdatePrepare();
            UpdateLoading();
        }

        protected void UpdatePrepare()
        {
            while (m_prepareQueue.Count > 0)
            {
                var itPair = m_prepareQueue.First.Value;

                itPair.Value.status = AssetLoadStatus.LOAD_LOADING;
                OnLoadAsset(itPair.Value);

                m_prepareQueue.RemoveFirst();
                m_loadQueue.AddLast(itPair);

                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
            }
        }

        protected void UpdateLoading()
        {
            foreach (var handlerPair in m_loadQueue)
            {
                if (IsCompleted(handlerPair.Value))
                {
                    m_loadQueue.Remove(handlerPair);
                }
            }
        }

        protected override int OnLoadingCount()
        {
            return m_prepareQueue.Count + m_loadQueue.Count;
        }

        protected void RemoveHandler(AssetLoadHandler handler)
        {
            foreach (var itPair in m_prepareQueue)
            {
                if (itPair.Key == handler.id)
                {
                    m_prepareQueue.Remove(itPair);
                    AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
                    break;
                }
            }

            foreach (var itPair in m_loadQueue)
            {
                if (itPair.Key == handler.id)
                {
                    m_loadQueue.Remove(itPair);
                    AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Value);
                    break;
                }
            }
        }

        protected virtual bool IsCompleted(AssetLoadHandler handler){ return true;}
        protected abstract IEnumerator OnLoadAsset(AssetLoadHandler handler);
    }
}

