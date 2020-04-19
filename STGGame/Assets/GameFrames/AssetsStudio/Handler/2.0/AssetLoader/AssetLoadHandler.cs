using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class AssetLoadHandler
    {
        public int id;
        public int status;
        public string path;
        public int timeout;
        public BaseLoader loader;


        private long m_tick;
        private AssetLoadCallback m_onCallback;
        private AssetLoadHandler m_parent;
        private List<AssetLoadHandler> m_children;

        public void AddChild(AssetLoadHandler handler)
        {
            if (handler.m_parent != null)
                return;
            
            handler.m_parent = this;
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

        public bool IsCompleted()
        {
            bool isCompleted = (status == AssetLoadStatus.LOAD_FINISH);
            if (m_children != null && m_children.Count > 0)
            {
                foreach(var loader in m_children)
                {
                    isCompleted &= loader.IsCompleted();
                }
            }
            return isCompleted;
        }

        //当且仅当子handler返回所有结果后才返回
        public void Invoke(AssetLoadResult result)
        {
            if (m_children != null && m_children.Count > 0)
            {
                //TODO:如果所有子都回调完成了,在回调

            }
            else
            {
                m_onCallback?.Invoke(result);
            }
        }

        public void Reset()
        {
            id = 0;
            status = 0;
            loader = null;
            path = null;

            m_onCallback = null;

            m_parent = null;
            m_children?.Clear();
        }
    }
}
