using System;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASGame
{
    public class AssetLoadHandlerManager : MonoSingleton<AssetLoadHandlerManager>
    {
        private static readonly float HANDLER_CLEAR_TIME = 15f;
        private int m_id;
        private LinkedList<AssetLoadHandler> m_availableHandlers = new LinkedList<AssetLoadHandler>();
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
                m_availableHandlers.AddLast(newHandler);

                newHandler.clearChecker.stayTime = HANDLER_CLEAR_TIME;
                newHandler.clearChecker.UpdateTick();
            }
            AssetLoadHandler handler = m_availableHandlers.Last.Value;
            m_availableHandlers.RemoveLast();

            handler.Reset();
            handler.Retain();
            handler.id = GetNewHandlerId();

            if (!m_handlerMaps.ContainsKey(handler.id))
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
                    m_availableHandlers.AddLast(handler);

                    handler.clearChecker.stayTime = HANDLER_CLEAR_TIME;
                    handler.clearChecker.UpdateTick();
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

        private void Update()
        {
            if (m_availableHandlers.Count > 0)
            {
                for (LinkedListNode<AssetLoadHandler> iterNode = m_availableHandlers.Last; iterNode != null; iterNode = iterNode.Previous)
                {
                    var handler = iterNode.Value;
                    if (handler.clearChecker.CheckTick())
                    {
                        m_availableHandlers.Remove(iterNode);
                    }
                }
            }
        }
    }
}
