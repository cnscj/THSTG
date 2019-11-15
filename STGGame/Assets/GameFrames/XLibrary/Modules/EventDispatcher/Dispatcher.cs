﻿using System.Collections.Generic;

namespace XLibGame
{

    /// <summary>
    /// 游戏事件分发器，用于监听、广播游戏事件。
    /// </summary>
    public class Dispatcher
    {
        
        private Dictionary<int, EventListener> m_listeners = new Dictionary<int, EventListener>();

        /// <summary>
        /// 广播指定事件。
        /// </summary>
        /// <param name="context">事件内容</param>
        public void Dispatch(EventContext context)
        {
            if (context == null) return;
            EventListener listener = null;
            if (m_listeners.TryGetValue(context.type,out listener))
            {
                listener.Call(context);
            }
        }

        /// <summary>
        /// 广播指定事件。
        /// </summary>
        /// <param name="type">事件编号</param>
        /// <param name="sender">发送者</param>
        /// <param name="args">事件参数</param>
        public void Dispatch(int type, object sender = null, params object[] args)
        {
            EventContext context = new EventContext(type, sender, args);
            Dispatch(context);
        }

        /// <summary>
        /// 添加对指定事件的监听。
        /// </summary>
        /// <param name="type">事件编号</param>
        /// <param name="callback0">回调委托</param>
        public void AddListener(int type, EventCallback0 callback0, int priority = 1)
        {
            EventListener listener = null;
            if (!m_listeners.TryGetValue(type, out listener))
            {
                listener = new EventListener();
                m_listeners.Add(type, listener);
            }
            listener.Add(callback0, priority);
        }

        /// <summary>
        /// 添加对指定事件的监听。
        /// </summary>
        /// <param name="type">事件编号</param>
        /// <param name="callback1">回调委托</param>
        public void AddListener(int type, EventCallback1 callback1, int priority = 1)
        {
            EventListener listener = null;
            if (!m_listeners.TryGetValue(type, out listener))
            {
                listener = new EventListener();
                m_listeners.Add(type, listener);
            }
            listener.Add(callback1, priority);
        }


        /// <summary>
        /// 移除对指定事件的监听。
        /// </summary>
        /// <param name="type">事件编号</param>
        /// <param name="callback0">回调委托</param>
        public void RemoveListener(int type, EventCallback0 callback0)
        {
            EventListener listener = null;
            if (m_listeners.TryGetValue(type, out listener))
            {
                listener.Remove(callback0);
            }
        }

        /// <summary>
        /// 移除对指定事件的监听。
        /// </summary>
        /// <param name="type">事件编号</param>
        /// <param name="callback1">回调委托</param>
        public void RemoveListener(int type, EventCallback1 callback1)
        {
            EventListener listener = null;
            if (m_listeners.TryGetValue(type, out listener))
            {
                listener.Remove(callback1);
            }
        }

        /// <summary>
        /// 清除所有的事件监听器。
        /// </summary>
        public void Clear()
        {
            m_listeners.Clear();
        }

        /// <summary>
        /// 是否有该事件的监听器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool HasListener(int type)
        {
            EventListener listener = null;
            if (m_listeners.TryGetValue(type, out listener))
            {
                return true;
            }
            return false;
        }

    }
}
