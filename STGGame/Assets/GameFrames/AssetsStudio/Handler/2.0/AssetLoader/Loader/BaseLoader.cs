using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public abstract class BaseLoader : MonoBehaviour
    {
        public int maxLoadingCount = -1;                            //最大同时加载资源个数
        private LinkedList<AssetLoadHandler> m_preloadQueue;        //预加载队列
        private LinkedList<AssetLoadHandler> m_waitQueue;           //准备队列
        private LinkedList<KeyValuePair<AssetLoadHandler, AssetLoadResult>> m_finishQueue;         //完成队列

        public void Preload(string []path)
        {

        }

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

        public void FinishHandler(AssetLoadHandler handler, AssetLoadResult result)
        {
            m_finishQueue = m_finishQueue ?? new LinkedList<KeyValuePair<AssetLoadHandler, AssetLoadResult>>();
            m_finishQueue.AddLast(new KeyValuePair<AssetLoadHandler, AssetLoadResult>(handler, result));
        }

        public void Clear()
        {
            OnClear();
        }

        public int GetLoadingCount()
        {
            return OnLoadingCount();
        }

        protected void Update()
        {
            UpdatePreload();
            UpdateWait();
            OnUpdate();
            UpdateFinish();
        }

        protected LinkedList<AssetLoadHandler> GetWaitQueue()
        {
            m_waitQueue = m_waitQueue ?? new LinkedList<AssetLoadHandler>();
            return m_waitQueue;
        }

        protected void UpdatePreload()
        {
            if (m_preloadQueue != null)
            {
                //加载队列空闲时,启动预加载
                if (OnLoadingCount() <= 0)
                {

                }
            }
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

        protected void UpdateFinish()
        {
            if (m_finishQueue != null)
            {
                while(m_finishQueue.Count > 0)
                {
                    var pair = m_finishQueue.First.Value;
                    var handler = pair.Key;
                    var result = pair.Value;
                    
                    handler.onCallback?.Invoke(result);

                    OnLoadCompleted(handler);
                    AssetLoadHandlerManager.GetInstance().RecycleHandler(handler);
                    m_finishQueue.RemoveFirst();
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
        protected virtual void OnLoadCompleted(AssetLoadHandler handler) { }
        protected abstract void OnClear();
    }
}

