using System;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASGame
{
    public class AssetLoadHandlerManager : MonoSingleton<AssetLoadHandlerManager>
    {
        private int m_id;
        private Queue<AssetLoadHandler> m_availableHandlers = new Queue<AssetLoadHandler>();
        private Dictionary<int, AssetLoadHandler> m_handlerMaps = new Dictionary<int, AssetLoadHandler>();

        public AssetLoadHandler GetLoadHandler(int id)
        {
            if (m_handlerMaps.ContainsKey(id))
            {
                return m_handlerMaps[id];
            }
            return null;
        }

        public int GetNewHandlerId() { return ++m_id; }

        public AssetLoadHandler GetOrCreateHandler()
        {
            if (m_availableHandlers.Count <= 0)
            {
                var newHandler = new AssetLoadHandler();
                newHandler.ReleaseLater();
                m_availableHandlers.Enqueue(newHandler);
            }
            AssetLoadHandler handler = m_availableHandlers.Dequeue();
            handler.Reset();
            handler.Retain();
            handler.id = GetNewHandlerId();

            if (m_handlerMaps.ContainsKey(handler.id))
            {
                m_handlerMaps.Add(handler.id, handler);
            }

            return handler;
        }

        public void RecycleHandler(AssetLoadHandler handler)
        {
            if (handler != null)
            {
                if (m_handlerMaps.ContainsKey(handler.id))
                {
                    m_handlerMaps.Remove(handler.id);
                    m_availableHandlers.Enqueue(handler);
                }
            }
        }

        public void ClearAll()
        {
            m_availableHandlers.Clear();
            foreach(var itPair in m_handlerMaps)
            {
                itPair.Value.loader?.StopLoad(itPair.Value);
            }
            m_handlerMaps.Clear();
            m_id = 0;
        }

    }
}
