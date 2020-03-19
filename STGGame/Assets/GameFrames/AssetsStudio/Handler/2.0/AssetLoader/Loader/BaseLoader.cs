using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public abstract class BaseLoader : MonoBehaviour
    {
        public int maxLoadingCount = -1;                            //最大加载资源个数
        private LinkedList<AssetLoadHandler> m_waitQueue;           //准备队列

        public AssetLoadHandler StartLoad(string path)
        {
            var handler = AssetLoadHandlerManager.GetInstance().GetOrCreateHandler();
            handler.path = path;
            if (maxLoadingCount > 0 && OnLoadingCount() >= maxLoadingCount)
            {
                handler.status = AssetLoadStatus.LOAD_WAIT;
                GetWaitQueue().AddLast(handler);
            }
            else
            {
                StartWithHandler(handler);
            }
            return handler;
        }

        public void StopLoad(AssetLoadHandler handler)
        {
            if (handler.status == AssetLoadStatus.LOAD_WAIT)
            {
                if (m_waitQueue != null)
                {
                    foreach (var waitHandler in m_waitQueue)
                    {
                        if (waitHandler == handler)
                        {
                            m_waitQueue.Remove(waitHandler);
                        }
                    }
                }
            }
            else
            {
                OnStopLoad(handler);
            }

        }

        public void Clear()
        {
            OnClear();
        }

        protected void Update()
        {
            UpdateWait();
            OnUpdate();
        }

        protected LinkedList<AssetLoadHandler> GetWaitQueue()
        {
            if (m_waitQueue == null)
            {
                m_waitQueue = new LinkedList<AssetLoadHandler>();
            }
            return m_waitQueue;
        }

        protected void UpdateWait()
        {
            if (m_waitQueue != null)
            {
                while (maxLoadingCount > 0 && OnLoadingCount() < maxLoadingCount && GetWaitQueue().Count > 0)
                {
                    var handler = GetWaitQueue().First.Value;
                    StartWithHandler(handler);
                    GetWaitQueue().RemoveFirst();
                }
            }
        }

        private void StartWithHandler(AssetLoadHandler handler)
        {
            handler.status = AssetLoadStatus.LOAD_LOADING;
            OnStartLoad(handler);
        }

        protected virtual void OnUpdate(){ }
        protected abstract int OnLoadingCount();
        protected abstract void OnStartLoad(AssetLoadHandler handler);
        protected abstract void OnStopLoad(AssetLoadHandler handler);
        protected abstract void OnClear();
    }
}

