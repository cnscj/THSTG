using System.Collections.Generic;
using XLibGame;

namespace ASGame
{
    public class AssetLoadHandler : BaseRef
    {
        public int id;
        public int status;
        public string path;
        public BaseLoader loader;

        private AssetLoadResult m_result;
        private AssetLoadCallback m_onCallback;
        private AssetLoadHandler m_parent;
        private List<AssetLoadHandler> m_children;
        private int m_callbackCount;

        public void AddChild(AssetLoadHandler handler)
        {
            if (handler.m_parent != null)
                return;
            
            handler.m_parent = this;
            handler.Retain();
            m_children = m_children ?? new List<AssetLoadHandler>();
            m_children.Add(handler);
        }

        public AssetLoadHandler[] GetChildren()
        {
            return m_children?.ToArray();
        }

        public void RemoveFromParent()
        {
            if (m_parent != null)
            {
                var childList = m_parent.m_children;
                if (childList != null && childList.Count > 0)
                {
                    for (int i = childList.Count - 1; i >= 0; i--)
                    {
                        var handler = childList[i];
                        if (handler == this)
                        {
                            childList.RemoveAt(i);
                            m_parent = null;
                            break;
                        }
                    }
                }
            }
        }

        public AssetLoadCallback OnCompleted(AssetLoadCallback callback = null)
        {
            if (m_onCallback != null && callback != null)
            {
                m_onCallback += callback;
            }
            return m_onCallback;
        }

        //当且仅当子handler返回所有结果后才返回
        public bool TryInvoke(AssetLoadResult result)
        {
            //这里得区分是自己的回调,还是由子回调引起的回调由此来确定result的正确性
            m_result = result ?? m_result;
            if (m_children != null && m_children.Count > 0)
            {
                if(m_callbackCount < m_children.Count)
                {
                    m_callbackCount++;
                    return false;
                }
            }

            m_onCallback?.Invoke(m_result);
            m_parent?.TryInvoke(null);
            return true;
        }

        public void Reset()
        {
            id = 0;
            status = 0;
            loader = null;
            path = null;

            m_result = null;
            m_onCallback = null;
            m_parent = null;
            m_children = null;
            m_callbackCount = 0;
        }

        protected override void OnRelease()
        {
            //递归释放
            if (m_children != null && m_children.Count > 0)
            {
                for (int i = m_children.Count - 1; i >= 0; i--)
                {
                    var handler = m_children[i];
                    handler.Release();
                }
            }
            AssetLoadHandlerManager.GetInstance().RecycleHandler(this);
        }
    }
}
