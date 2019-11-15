using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public class EventTrigger : MonoBehaviour
    {
        [Header("派发器名称")]
        public string dispatchr;

        [Header("事件名称")]
        public string evt;

        [Header("事件数据")]
        public string data;

        [Header("触发")]
        public bool isTrigger = false;

        public void Dispatch()
        {
            var dispatcher = DispatcherManager.GetInstance().GetDispatcher(dispatchr);
            if (dispatcher != null)
            {
                if (!string.IsNullOrEmpty(evt))
                {
                    dispatcher.Dispatch(evt.GetHashCode(), this, data);
                }
            }
        }

        private void Update()
        {
            if (isTrigger)
            {
                Dispatch();
                isTrigger = false;
            }
        }
    }
}

