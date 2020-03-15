using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ASGame
{
    public abstract class BaseLoader : MonoBehaviour
    {
        private static int s_id;
        private static Queue<AssetLoadHandler> s_availableHandlers = new Queue<AssetLoadHandler>();

        protected static int GetNewHandlerId(){return ++s_id;}

        public abstract AssetLoadHandler StartLoad(string path);
        public abstract void StopLoad(AssetLoadHandler handler);
        
        
        protected AssetLoadHandler GetOrCreateHandler()
        {
            if (s_availableHandlers.Count <= 0 )
            {
                var newHandler = new AssetLoadHandler();
                s_availableHandlers.Enqueue(newHandler);
            }
            AssetLoadHandler handler = s_availableHandlers.Dequeue();
            handler.id = GetNewHandlerId();

            return handler;
        }
        protected void RecycleHandler(AssetLoadHandler handler)
        {
            if (handler != null)
            {
                s_availableHandlers.Enqueue(handler);
                handler.id = 0;
                handler.loader = null;
                handler.assetPath = null;
                handler.onCallback = null;
            }
        }
    }
}

