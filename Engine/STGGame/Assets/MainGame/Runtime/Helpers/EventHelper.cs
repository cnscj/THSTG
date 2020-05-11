using System.Collections.Generic;
using STGU3D;
using XLibGame;
using DispatcherManager = STGU3D.DispatcherManager;
namespace STGRuntime
{
    public static class EventHelper
    {

        /// <summary>
        /// 广播指定事件。
        /// </summary>
        /// <param name="eventId">事件编号</param>
        /// <param name="args">事件参数</param>
        public static void Dispatch(int eventId, object sender = null, params object[] args)
        {
            DispatcherManager.GetInstance().Dispatch(eventId, sender, args);
        }

        /// <summary>
        /// 添加对指定事件的监听。
        /// </summary>
        /// <param name="eventId">事件编号</param>
        /// <param name="listener">回调委托</param>
        public static void AddListener(int eventId, EventCallback0 listener, int priority = 1)
        {
            DispatcherManager.GetInstance().AddListener(eventId, listener, priority);
        }

        /// <summary>
        /// 添加对指定事件的监听。
        /// </summary>
        /// <param name="eventId">事件编号</param>
        /// <param name="listener">回调委托</param>
        public static void AddListener(int eventId, EventCallback1 listener, int priority = 1)
        {
            DispatcherManager.GetInstance().AddListener(eventId, listener, priority);
        }


        /// <summary>
        /// 移除对指定事件的监听。
        /// </summary>
        /// <param name="eventId">事件编号</param>
        /// <param name="callback0">回调委托</param>
        public static void RemoveListener(int eventId, EventCallback0 callback0)
        {
            DispatcherManager.GetInstance().RemoveListener(eventId, callback0);
        }

        /// <summary>
        /// 移除对指定事件的监听。
        /// </summary>
        /// <param name="eventId">事件编号</param>
        /// <param name="callback1">回调委托</param>
        public static void RemoveListener(int eventId, EventCallback1 callback1)
        {
            DispatcherManager.GetInstance().RemoveListener(eventId, callback1);
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
