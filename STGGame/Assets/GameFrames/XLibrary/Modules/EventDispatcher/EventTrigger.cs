using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public class EventTrigger : MonoBehaviour
    {
        public static string defaultDispatcherName = "default";

        [Header("派发器名称")]
        public string dispatchr = defaultDispatcherName;

        [Header("事件名称")]
        public string evt;

        [Header("事件数据")]
        public string data;

        public void Dispatch()
        {
            if (!string.IsNullOrEmpty(evt))
            {
                var dispatcher = DispatcherManager.GetInstance().GetOrNewDispatcher(dispatchr);
                if (dispatcher != null)
                {
                    dispatcher.Dispatch(evt.GetHashCode(), this, data);
                }
            }
        }
    }
}

