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

        private LinkedList<AssetLoadHandler> m_successQueue;        //成功队列
        private LinkedList<AssetLoadHandler> m_failedQueue;         //失败队列
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

        public virtual void UnLoad(string path)
        {

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
            UpdateSuccess();
            UpdateFailed();

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

        //资源加载成功
        protected void LoadHandlerSuccess(AssetLoadHandler handler)
        {

            OnLoadSuccess(handler);
        }

        //资源加载失败
        protected void LoadHandlerFailed(AssetLoadHandler handler)
        {
            //资源加载失败

            //TODO:释放掉所有的依赖加载器

            OnLoadFailed(handler);
        }

        ////////////
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

        private LinkedList<AssetLoadHandler> GetSuccessQueue()
        {
            m_successQueue = m_successQueue ?? new LinkedList<AssetLoadHandler>();
            return m_successQueue;
        }

        private LinkedList<AssetLoadHandler> GetFailedQueue()
        {
            m_failedQueue = m_failedQueue ?? new LinkedList<AssetLoadHandler>();
            return m_failedQueue;
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
                    //状态更新
                    if (handler.status == AssetLoadStatus.LOAD_LOADING)
                    {
                        //超时检测
                        if (handler.CheckTimeout())
                        {
                            handler.status = AssetLoadStatus.LOAD_TIMEOUT;
                        }
                    }


                    //全部只有两种结果,要么成功,要么失败
                    if (handler.status == AssetLoadStatus.LOAD_FINISHED)
                    {
                        GetSuccessQueue().AddLast(handler);
                    }
                    else if (handler.status < AssetLoadStatus.LOAD_IDLE)
                    {
                        GetFailedQueue().AddLast(handler);
                    }
                }
            }
        }

        //送入缓存,并返回
        private void UpdateSuccess()
        {
            if (m_successQueue != null)
            {
                while (m_successQueue.Count > 0)
                {
                    var handler = m_successQueue.First.Value;
                    LoadHandlerSuccess(handler);
     
                    m_loadingMap.Remove(handler.path);
                    m_successQueue.RemoveFirst();
                    GetReleaseQueue().AddLast(handler);
                }
            }
        }

        //释放所有依赖,并返回
        private void UpdateFailed()
        {
            if (m_failedQueue != null)
            {
                while (m_failedQueue.Count > 0)
                {
                    var handler = m_failedQueue.First.Value;
                    LoadHandlerFailed(handler);

                    m_loadingMap.Remove(handler.path);
                    m_failedQueue.RemoveFirst();
                    GetReleaseQueue().AddLast(handler);
                }
            }
        }

        //如果外部不持有Handler,释放
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
        protected virtual void OnLoadSuccess(AssetLoadHandler handler) { }
        protected virtual void OnLoadFailed(AssetLoadHandler handler) { }
    }
}

