﻿using System.Collections.Generic;

namespace XLibGame
{

    /// <summary>
    /// 游戏事件分发器，用于监听、广播游戏事件。
    /// </summary>
    public class Dispatcher
    {
        public delegate void EventListener(int eventId, Dictionary<string, object> args);
        private Dictionary<int, SortedList<int,EventListener>> m_listeners = new Dictionary<int, SortedList<int, EventListener>>();

        /// <summary>
        /// 广播指定事件。
        /// </summary>
        /// <param name="eventId">事件编号</param>
        /// <param name="args">事件参数</param>
        public void Dispatch(int eventId, Dictionary<string, object> args = null)
        {
            var listeners = GetEventListeners(eventId);
            foreach(var pair in listeners)
            {
                var listener = pair.Value;
                listener.Invoke(eventId, args);
            }
        }

        /// <summary>
        /// 添加对指定事件的监听。
        /// </summary>
        /// <param name="eventId">事件编号</param>
        /// <param name="listener">回调委托</param>
        public void AddListener(int eventId, EventListener listener, int priority = 1)
        {
            var listeners = GetEventListeners(eventId);
            listeners.Add(priority,listener);
        }

        /// <summary>
        /// 移除对指定事件的监听。
        /// </summary>
        /// <param name="eventId">事件编号</param>
        /// <param name="listenerToBeRemoved">回调委托</param>
        public void RemoveListener(int eventId, EventListener listenerToBeRemoved)
        {
            var listeners = GetEventListeners(eventId);
            for (var i = 0; i < listeners.Count; i++)
            {
                var listener = listeners[i];
                if (listener == listenerToBeRemoved)
                {
                    listeners.RemoveAt(i);
                    break;
                }
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
        /// <param name="eventId"></param>
        /// <returns></returns>
        public bool HasListener(int eventId)
        {
            var listeners = GetEventListeners(eventId);
            return listeners.Count > 0;
        }

        private SortedList<int, EventListener> GetEventListeners(int eventId)
        {
            SortedList<int, EventListener> ret;
            if (m_listeners.TryGetValue(eventId, out ret))
                return ret;
            ret = new SortedList<int, EventListener>();
            m_listeners.Add(eventId, ret);
            return ret;
        }

       
    }
}