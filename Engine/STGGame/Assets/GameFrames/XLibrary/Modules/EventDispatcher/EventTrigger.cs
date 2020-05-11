using UnityEngine;

namespace XLibGame
{
    public class EventTrigger : MonoBehaviour
    {
        public static string defaultDispatcherName = "default";

        [Header("派发器名称")]
        public string dispatchr = defaultDispatcherName;

        [Header("事件类型")]
        public string evt;

        [HideInInspector]
        private Dispatcher GetDispatcher()
        {
            return DispatcherManager.GetInstance().GetOrNewDispatcher(dispatchr);
        }

        public void DispatchEvent(int e)
        {
            GetDispatcher().Dispatch(e, this);
        }

        public void Dispatch()
        {
            GetDispatcher().Dispatch(evt, this);
        }

        public void DispatchString(string obj)
        {
            GetDispatcher().Dispatch(evt, this, obj);
        }

        public void DispatchObject(Object obj)
        {
            GetDispatcher().Dispatch(evt, this, obj);
        }


    }
}