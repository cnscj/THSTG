using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public abstract class BaseLoader : MonoBehaviour, IAssetLoader
    {
        public int maxLoadingCount = -1;                            //最大同时加载资源个数
        private LinkedList<AssetLoadHandler> m_waitQueue;           //等待队列
        private Dictionary<string, AssetLoadHandler> m_loadingMap;  //加载队列
        private LinkedList<AssetLoadHandler> m_finishQueue;         //完成队列
        private LinkedList<AssetLoadHandler> m_abortedQueue;        //中断队列
        private LinkedList<AssetLoadHandler> m_releaseQueue;        //释放队列

        public int WaitingCount
        {
            get{ return m_waitQueue != null ? m_waitQueue.Count : 0; }
        }

        public int LoadingCount
        {
            get { return m_loadingMap != null ? m_loadingMap.Count : 0; }
        }

        public virtual AssetLoadHandler StartLoad(string path)
        {
            var handler = GetOrCreateHandler(path);

            //等待队列里如果有依赖加载器,会引起主加载器没法完成,造成无限等待,所以依赖加载不能走这里
            if (maxLoadingCount > 0 && LoadingCount >= maxLoadingCount)
            {
                handler.status = AssetLoadStatus.LOAD_WAIT;
                GetWaitQueue().AddLast(handler);
            }
            else
            {
                StartLoadWithHandler(handler);
            }
            return handler;
        }

        public virtual void StopLoad(AssetLoadHandler handler)
        {
            if (handler.status == AssetLoadStatus.LOAD_WAIT)
            {
                if (m_waitQueue != null && m_waitQueue.Count > 0)
                {
                    for (LinkedListNode<AssetLoadHandler> iterNode = m_waitQueue.Last; iterNode != null; iterNode = iterNode.Previous)
                    {
                        var waitHandler = iterNode.Value;
                        if (waitHandler == handler)
                        {
                            m_waitQueue.Remove(iterNode);
                            GetReleaseQueue().AddLast(handler);
                            break;
                        }
                    }
                }
            }
            else
            {
                StopLoadWithHandler(handler);
            }
        }

        public virtual void Clear()
        {
            m_waitQueue?.Clear();
            if (m_loadingMap != null)
            {
                foreach(var handler in m_loadingMap.Values)
                {
                    StopLoad(handler);
                }
                m_loadingMap.Clear();
            }
        }

        protected void Update()
        {
            UpdateWait();
            OnUpdate();
            UpdateStatus();
            UpdateFinish();
            UpdateAborted();
            UpdateRelease();
        }

        protected AssetLoadHandler GetOrCreateHandler(string path)
        {
            //先判断正作加载的队列中是否已经正在加载
            AssetLoadHandler handler = null;
            if (m_loadingMap != null && m_loadingMap.TryGetValue(path, out handler))
            {
                return handler;
            }

            handler = AssetLoadHandlerManager.GetInstance().GetOrCreateHandler();
            handler.path = path;
            return handler;
        }

        protected void StartLoadWithHandler(AssetLoadHandler handler)
        {
            var loadingMap = GetLoadingMap();
            if (!loadingMap.ContainsKey(handler.path))
            {
                loadingMap.Add(handler.path, handler);
                handler.status = AssetLoadStatus.LOAD_LOADING;
                OnStartLoad(handler);
            }
        }

        protected void StopLoadWithHandler(AssetLoadHandler handler)
        {
            handler.status = AssetLoadStatus.LOAD_ABORT;
            OnStopLoad(handler);
            
        }

        private LinkedList<AssetLoadHandler> GetWaitQueue()
        {
            m_waitQueue = m_waitQueue ?? new LinkedList<AssetLoadHandler>();
            return m_waitQueue;
        }

        private Dictionary<string, AssetLoadHandler> GetLoadingMap()
        {
            m_loadingMap = m_loadingMap ?? new Dictionary<string, AssetLoadHandler>();
            return m_loadingMap;
        }

        private LinkedList<AssetLoadHandler> GetFinishQueue()
        {
            m_finishQueue = m_finishQueue ?? new LinkedList<AssetLoadHandler>();
            return m_finishQueue;
        }

        private LinkedList<AssetLoadHandler> GetAbortQueue()
        {
            m_abortedQueue = m_abortedQueue ?? new LinkedList<AssetLoadHandler>();
            return m_abortedQueue;
        }

        private LinkedList<AssetLoadHandler> GetReleaseQueue()
        {
            m_releaseQueue = m_releaseQueue ?? new LinkedList<AssetLoadHandler>();
            return m_releaseQueue;
        }

        private void UpdateWait()
        {
            if (m_waitQueue != null)
            {
                while (maxLoadingCount > 0 && LoadingCount < maxLoadingCount && WaitingCount > 0)
                {
                    var handler = GetWaitQueue().First.Value;
                    StartLoadWithHandler(handler);
                    GetWaitQueue().RemoveFirst();
                }
            }
        }

        private void UpdateStatus()
        {
            if (m_loadingMap != null)
            {
                foreach(var handler in m_loadingMap.Values)
                {
                    if (handler.status == AssetLoadStatus.LOAD_LOADING)
                    {
                        if (handler.CheckTimeout())
                        {
                            handler.status = AssetLoadStatus.LOAD_TIMEOUT;
                        }
                    }

                    if (handler.status == AssetLoadStatus.LOAD_FINISHED)
                    {
                        GetFinishQueue().AddLast(handler);
                    }
                    else if (
                        handler.status == AssetLoadStatus.LOAD_ABORT ||
                        handler.status == AssetLoadStatus.LOAD_TIMEOUT)
                    {
                        GetAbortQueue().AddLast(handler);
                    }
                }
            }
        }

        private void UpdateFinish()
        {
            if (m_finishQueue != null)
            {
                while (m_finishQueue.Count > 0)
                {
                    var handler = m_finishQueue.First.Value;
                    OnLoadCompleted(handler);
     
                    m_loadingMap.Remove(handler.path);
                    m_finishQueue.RemoveFirst();
                    GetReleaseQueue().AddLast(handler);
                }
            }
        }

        private void UpdateAborted()
        {
            if (m_abortedQueue != null)
            {
                while (m_abortedQueue.Count > 0)
                {
                    var handler = m_abortedQueue.First.Value;
                    OnLoadAborted(handler);

                    m_loadingMap.Remove(handler.path);
                    m_abortedQueue.RemoveFirst();
                    GetReleaseQueue().AddLast(handler);
                }
            }
        }

        private void UpdateRelease()
        {
            if (m_releaseQueue != null)
            {
                while (m_releaseQueue.Count > 0)
                {
                    var handler = m_releaseQueue.First.Value;

                    handler.ReleaseLater();
                    m_releaseQueue.RemoveFirst();
                }
            }
        }

        protected virtual void OnUpdate(){ }
        protected abstract void OnStartLoad(AssetLoadHandler handler);
        protected abstract void OnStopLoad(AssetLoadHandler handler);
        protected virtual void OnLoadCompleted(AssetLoadHandler handler) { }
        protected virtual void OnLoadAborted(AssetLoadHandler handler) { }
    }
}

