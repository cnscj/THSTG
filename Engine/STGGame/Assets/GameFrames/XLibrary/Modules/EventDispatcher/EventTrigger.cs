using UnityEngine;

namespace XLibGame
{
    public class EventTrigger : MonoBehaviour
    {

        [Header("事件类型")]
        public string evt;

        [HideInInspector]
        private EventDispatcher GetDispatcher()
        {
            return EventDispatcher.GetInstance();
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