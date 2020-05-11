using System;
using System.Collections.Generic;
using System.Linq;
using XLibGame;

namespace ASGame
{
    public class AssetLoadHandler : BaseRef
    {
        public int id;
        public int status;
        public string path;
        public AssetLoadResult result;
        public AssetLoadCallback onCallback;
        public BaseLoader loader;

        public Timechecker timeoutChecker = new Timechecker();
        public Timechecker clearChecker = new Timechecker();

        private HashSet<AssetLoadHandler> m_parents;
        private List<AssetLoadHandler> m_children;
        private int m_callbackCount;
        private Action<AssetLoadHandler> m_callbackCall;

        public void Abort()
        {
            loader?.StopLoad(this);
        }

        public void AddChild(AssetLoadHandler handler)
        {
            handler.m_parents = handler.m_parents ?? new HashSet<AssetLoadHandler>();
            m_children = m_children ?? new List<AssetLoadHandler>();

            if (handler.m_parents.Contains(this))
                return;

            handler.Retain();

            handler.m_parents.Add(this);
            m_children.Add(handler);
        }

        public AssetLoadHandler[] GetChildren()
        {
            return m_children?.ToArray();
        }

        public AssetLoadHandler[] GetParents()
        {
            return m_parents?.ToArray();
        }

        public void OnCompleted(AssetLoadCallback callback = null)
        {
            if (callback != null)
            {
                onCallback += callback;
            }
        }

        //有结果返回就是完成,与成功失败无关
        public bool IsCompleted()
        {
            bool isCompleted = (result != null);
            if (m_children != null && m_children.Count > 0)
            {
                foreach(var child in m_children)
                {
                    if (!child.IsCompleted())
                    {
                        isCompleted = false;
                        break;
                    }
                }
            }
            return isCompleted;
        }

        //是否存在加载出错的项
        public bool IsHadError()
        {
            bool isHadError = (status < AssetLoadStatus.LOAD_IDLE);
            if (m_children != null && m_children.Count > 0)
            {
                foreach (var child in m_children)
                {
                    if (!child.IsHadError())
                    {
                        isHadError = true;
                        break;
                    }
                }
            }
            return isHadError;
        }

        public void Callback(AssetLoadResult ret)
        {
            result = ret ?? result;
            onCallback?.Invoke(result);
        }

        public void TryCallback(AssetLoadResult ret = null, Action<AssetLoadHandler> callback = null)
        {
            //这里得区分是自己的回调,还是由子回调引起的回调由此来确定result的正确性
            result = ret ?? result;
            m_callbackCall = m_callbackCall ?? callback;

            if (m_children != null && m_children.Count > 0)
            {
                if (m_callbackCount < m_children.Count)   //所有子回调
                {
                    m_callbackCount++;
                }
                else
                {
                    if (result == null)
                    {
                        return;
                    }
                }
            }

            m_callbackCall?.Invoke(this);
            Callback(result);

            if (m_parents != null && m_parents.Count > 0)
            {
                foreach (var parent in m_parents)
                {
                    parent?.TryCallback(ret, callback);
                }
            }
        }

        public void Reset()
        {
            id = 0;
            status = 0;
            result = null;
            loader = null;
            path = null;

            onCallback = null;

            m_parents?.Clear();
            m_children?.Clear();
            m_callbackCount = 0;
            m_callbackCall = null;
        }

        protected override void OnRelease()
        {
            //递归释放
            if (m_children != null && m_children.Count > 0)
            {
                for (int i = m_children.Count - 1; i >= 0; i--)
                {
                    var handler = m_children[i];
                    handler.m_parents?.Remove(this);
                    handler.Release();
                }
            }
            AssetLoadHandlerManager.GetInstance().RecycleHandler(this);
        }

    }
}
