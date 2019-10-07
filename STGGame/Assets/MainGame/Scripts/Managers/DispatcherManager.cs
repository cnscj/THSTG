using UnityEngine;
using System.Collections;
using XLibGame;
namespace STGGame
{
    public static class DispatcherManager
    {
        private static Dispatcher s_dispatcher;

        public static Dispatcher GetInstance()
        {
            if (s_dispatcher == null)
            {
                s_dispatcher = new Dispatcher();
            }
            return s_dispatcher;
        }
    }

}

