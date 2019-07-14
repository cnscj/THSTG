using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace THGame
{
    public static class DispatcherManager
    {
        private static int s_dispatcherId = 10000;
        private static Dictionary<int, Dispatcher> s_dispatcherMap = new Dictionary<int, Dispatcher>();

        public static KeyValuePair<int, Dispatcher> NewDispatcher()
        {
            
            Dispatcher dispatcher = new Dispatcher();
            KeyValuePair<int, Dispatcher> pair = new KeyValuePair<int, Dispatcher>(s_dispatcherId++, dispatcher);
            s_dispatcherMap.Add(pair.Key, pair.Value);

            return pair;
        }

    }
}


