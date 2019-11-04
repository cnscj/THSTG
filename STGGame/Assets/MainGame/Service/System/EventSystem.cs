using System.Collections.Generic;
using STGU3D;
using XLibGame;

namespace STGService
{
    public static class EventSystem
    {
        /// <summary>
        /// 广播指定事件。
        /// </summary>
        /// <param name="eventId">事件编号</param>
        /// <param name="args">事件参数</param>
        public static void Dispatch(int eventId, Dictionary<string, object> args = null)
        {
            DispatcherManager.GetInstance().Dispatch(eventId, args);
        }

        /// <summary>
        /// 添加对指定事件的监听。
        /// </summary>
        /// <param name="eventId">事件编号</param>
        /// <param name="listener">回调委托</param>
        public static void AddListener(int eventId, EventListener2 listener, int priority = 1)
        {
            DispatcherManager.GetInstance().AddListener(eventId, listener, priority);
        }

        /// <summary>
        /// 移除对指定事件的监听。
        /// </summary>
        /// <param name="eventId">事件编号</param>
        /// <param name="listenerToBeRemoved">回调委托</param>
        public static void RemoveListener(int eventId, EventListener2 listenerToBeRemoved)
        {

            DispatcherManager.GetInstance().RemoveListener(eventId, listenerToBeRemoved);
        }

        /// <summary>
        /// 清除所有的事件监听器。
        /// </summary>
        public static void Clear()
        {
            DispatcherManager.GetInstance().Clear();
        }

        /// <summary>
        /// 是否有该事件的监听器
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public static bool HasListener(int eventId)
        {
            return DispatcherManager.GetInstance().HasListener(eventId);
        }

    }

}
