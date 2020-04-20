using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public abstract class BaseLoader : MonoBehaviour
    {
        public int maxLoadingCount = -1;                            //最大同时加载资源个数
        private LinkedList<AssetLoadHandler> m_waitQueue;           //等待队列
        private Dictionary<string, AssetLoadHandler> m_loadingMap;  //加载队列
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
                            AssetLoadHandlerManager.GetInstance().RecycleHandler(waitHandler);
                            break;
                        }
                    }
                }
            }
            else
            {
                OnStopLoad(handler);
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

        protected LinkedList<AssetLoadHandler> GetWaitQueue()
        {
            m_waitQueue = m_waitQueue ?? new LinkedList<AssetLoadHandler>();
            return m_waitQueue;
        }

        protected Dictionary<string,AssetLoadHandler> GetLoadingMap()
        {
            m_loadingMap = m_loadingMap ?? new Dictionary<string, AssetLoadHandler>();
            return m_loadingMap;
        }

        public LinkedList<AssetLoadHandler> GetReleaseQueue()
        {
            m_releaseQueue = m_releaseQueue ?? new LinkedList<AssetLoadHandler>();
            return m_releaseQueue;
        }

        protected void UpdateWait()
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

        protected void UpdateStatus()
        {
            if (m_loadingMap != null)
            {
                foreach(var handler in m_loadingMap.Values)
                {
                    if (handler.status == AssetLoadStatus.LOAD_FINISH)
                    {
                        OnLoadCompleted(handler);
                        GetReleaseQueue().AddLast(handler);
                    }
                    else if(handler.status == AssetLoadStatus.LOAD_TIMEOUT)
                    {
                        GetReleaseQueue().AddLast(handler);
                    }
                }
            }
        }

        protected void UpdateRelease()
        {
            if (m_releaseQueue != null)
            {
                while (m_releaseQueue.Count > 0)
                {
                    var handler = m_releaseQueue.First.Value;
                    m_loadingMap.Remove(handler.path);

                    AssetLoadHandlerManager.GetInstance().RecycleHandler(handler);
                    m_releaseQueue.RemoveFirst();
                }
            }
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

        protected virtual void OnUpdate(){ }
        protected abstract void OnStartLoad(AssetLoadHandler handler);
        protected abstract void OnStopLoad(AssetLoadHandler handler);
        protected virtual void OnLoadCompleted(AssetLoadHandler handler) { }
    }
}

