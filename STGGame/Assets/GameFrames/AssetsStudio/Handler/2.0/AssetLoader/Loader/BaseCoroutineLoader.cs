﻿using System;
using System.Collections.Generic;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;
namespace ASGame
{
    public abstract class BaseCoroutineLoader : BaseLoader
    {
        private Dictionary<int, KeyValuePair<AssetLoadHandler, Coroutine>> m_loadCoroutines = new Dictionary<int, KeyValuePair<AssetLoadHandler, Coroutine>>();  //正在加载的队列

        protected override void OnStartLoad(AssetLoadHandler handler)
        {
            handler.loader = this;

            var coroutine = StartCoroutine(LoadAssetCoroutine(handler));
            m_loadCoroutines[handler.id] = new KeyValuePair<AssetLoadHandler, Coroutine>(handler, coroutine);
        }

        protected override void OnStopLoad(AssetLoadHandler handler)
        {
            if (handler == null)
                return;

            if (m_loadCoroutines.ContainsKey(handler.id))
            {
                var itPair = m_loadCoroutines[handler.id];
                StopCoroutine(itPair.Value);
                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Key);
                m_loadCoroutines.Remove(handler.id);
            }
        }
        protected override void OnLoadCompleted(AssetLoadHandler handler)
        {
            m_loadCoroutines.Remove(handler.id);
        }

        protected override void OnClear()
        {
            foreach(var mapPair in m_loadCoroutines)
            {
                var itPair = mapPair.Value;
                StopCoroutine(itPair.Value);
                AssetLoadHandlerManager.GetInstance().RecycleHandler(itPair.Key);
            }
            m_loadCoroutines.Clear();
        }

        private IEnumerator LoadAssetCoroutine(AssetLoadHandler handler)
        {
            yield return OnLoadAsset(handler);
        }

        protected override int OnLoadingCount()
        {
            return m_loadCoroutines.Count;
        }

        protected abstract IEnumerator OnLoadAsset(AssetLoadHandler handler);
    }
}
