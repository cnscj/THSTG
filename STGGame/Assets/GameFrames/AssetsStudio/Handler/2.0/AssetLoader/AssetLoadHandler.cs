using System;
using UnityEngine;

namespace ASGame
{
    public class AssetLoadHandler
    {
        public int id;
        public int status;
        public string path;
        public int timeout = -1;
        public BaseLoader loader;
        public AssetLoadCallback onCallback;

        private long m_tick;
        private AssetLoadHandler m_parent;
        private AssetLoadHandler[] m_children;

        public bool IsCompleted()
        {
            bool isCompleted = (status == AssetLoadStatus.LOAD_SUCCESS);
            if (m_children != null)
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
            if (m_children != null && m_children.Length > 0)
            {

            }
            else
            {
                //不管有没有都通知下
            }
        }

        public void Reset()
        {
            id = 0;
            status = 0;
            loader = null;
            path = null;

            onCallback = null;

            m_parent = null;
            m_children = null;
        }
    }
}
