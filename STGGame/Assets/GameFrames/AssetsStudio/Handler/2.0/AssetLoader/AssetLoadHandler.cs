﻿using System.Collections.Generic;
using UnityEngine;
using XLibGame;

namespace ASGame
{
    public class AssetLoadHandler : BaseRef
    {
        public int id;
        public int status;
        public string path;
        public AssetLoadResult result;
        public BaseLoader loader;
        public float stayTime = 120f;

        private float m_updateTick;

        private AssetLoadCallback m_onCallback;
        private HashSet<AssetLoadHandler> m_parents;
        private List<AssetLoadHandler> m_children;
        private int m_callbackCount;

        public void AddChild(AssetLoadHandler handler)
        {
            m_parents = m_parents ?? new HashSet<AssetLoadHandler>();
            m_children = m_children ?? new List<AssetLoadHandler>();

            if (m_parents.Contains(this))
                return;

            handler.Retain();

            m_parents.Add(this);
            m_children.Add(handler);
        }

        public AssetLoadHandler[] GetChildren()
        {
            return m_children?.ToArray();
        }

        public AssetLoadCallback OnCompleted(AssetLoadCallback callback = null)
        {
            if (callback != null)
            {
                m_onCallback += callback;
            }
            return m_onCallback;
        }



        //当且仅当子handler返回所有结果后才返回
        public bool Transmit(AssetLoadResult ret)
        {
            //这里得区分是自己的回调,还是由子回调引起的回调由此来确定result的正确性
            result = ret ?? result;
            if (m_children != null && m_children.Count > 0)
            {
                if (m_callbackCount < m_children.Count)   //所有子回调
                {
                    m_callbackCount++;
                    return false;
                }
                else
                {
                    if (result == null)
                    {
                        return false;
                    }
                }  
            }

            if (m_parents != null && m_parents.Count > 0)
            {
                foreach (var parent in m_parents)
                {
                    parent?.Transmit(null);
                }
            }

            return true;
        }

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

        public void Callback()
        {
            m_onCallback?.Invoke(result);
        }

        public bool CheckTimeout()
        {
            if (m_updateTick + stayTime <= Time.realtimeSinceStartup)
            {
                return true;
            }
            return false;
        }

        public void UpdateTick()
        {
            m_updateTick = Time.realtimeSinceStartup;
        }

        public void Reset()
        {
            id = 0;
            status = 0;
            result = null;
            loader = null;
            path = null;
            stayTime = 120f;


            m_onCallback = null;
            m_parents?.Clear();
            m_children?.Clear();
            m_callbackCount = 0;

            UpdateTick();
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
