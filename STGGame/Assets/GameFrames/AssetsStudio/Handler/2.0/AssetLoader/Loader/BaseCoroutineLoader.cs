using System.Collections.Generic;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;
namespace ASGame
{
    public abstract class BaseCoroutineLoader : BaseLoader
    {
        private Dictionary<int, KeyValuePair<AssetLoadHandler, Coroutine>> m_loadCoroutines = new Dictionary<int, KeyValuePair<AssetLoadHandler, Coroutine>>();  //正在加载的队列
        private Dictionary<int, KeyValuePair<AssetLoadHandler, Coroutine>> m_loadNextframes = new Dictionary<int, KeyValuePair<AssetLoadHandler, Coroutine>>();  //下一帧返回的队列


        public override void Clear()
        {
            base.Clear();
           
            m_loadCoroutines.Clear();
            m_loadNextframes.Clear();
        }

        //TODO:优化,如果存在加载过的,直接下一帧返回,不需要启动协程
        protected override void OnStartLoad(AssetLoadHandler handler)
        {
            if (handler == null)
                return;

            LoadStartByCoroutine(handler);
        }

        protected override void OnStopLoad(AssetLoadHandler handler)
        {
            if (handler == null)
                return;

            LoadStopByCoroutine(handler);
        }

        private void LoadStartByCoroutine(AssetLoadHandler handler)
        {
            if (!m_loadCoroutines.ContainsKey(handler.id))
            {
                handler.loader = this;

                var coroutine = StartCoroutine(LoadAssetCoroutine(handler));
                m_loadCoroutines[handler.id] = new KeyValuePair<AssetLoadHandler, Coroutine>(handler, coroutine);
            }
        }

        private void LoadStopByCoroutine(AssetLoadHandler handler)
        {
            if (m_loadCoroutines.ContainsKey(handler.id))
            {
                var itPair = m_loadCoroutines[handler.id];
                StopCoroutine(itPair.Value);

                m_loadCoroutines.Remove(handler.id);
            }
        }

        private void LoadStartByNextframe(AssetLoadHandler handler)
        {

        }

        private void LoadStopByNextframe(AssetLoadHandler handler)
        {

        }

        protected override void OnLoadSuccess(AssetLoadHandler handler)
        {
            m_loadCoroutines.Remove(handler.id);
            m_loadNextframes.Remove(handler.id);
        }

        protected override void OnLoadFailed(AssetLoadHandler handler)
        {
            m_loadCoroutines.Remove(handler.id);
            m_loadNextframes.Remove(handler.id);
        }

        private IEnumerator LoadAssetCoroutine(AssetLoadHandler handler)
        {
            yield return OnLoadAsset(handler);
        }

        protected abstract IEnumerator OnLoadAsset(AssetLoadHandler handler);
    }
}

