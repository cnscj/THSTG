using UnityEngine;
using System.Collections;
using XLibGame;
namespace STGU3D
{
    public static class DispatcherManager
    {
        private static EventDispatcher s_dispatcher;

        public static EventDispatcher GetInstance()
        {
            if (s_dispatcher == null)
            {
                s_dispatcher = new EventDispatcher();
            }
            return s_dispatcher;
        }
    }

}

