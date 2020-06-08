using System;
using System.Collections.Generic;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;
namespace ASGame
{
    public abstract class BaseModeLoader : BaseLoader
    {
        protected class Nextframe
        {
            public bool isExecuted;
            public Action action;
        }

        protected enum LoadMode
        {
            //同步
            Immediately,    //TODO:立刻(可能无法设置回调,待讨论

            //异步
            Coroutine,      //协程
            Nextframe,      //下一帧
        }

        protected class LoadNode
        {
            public LoadMode loadMode;
            public Coroutine coroutine;
            public Nextframe nextframe;

            public void Reset()
            {
                loadMode = LoadMode.Coroutine;
                coroutine = null;
                nextframe = null;
            }
        }
    
        private Dictionary<int, LoadNode> m_loadNodes = new Dictionary<int, LoadNode>();
        private HashSet<Nextframe> m_nextframeMap = new HashSet<Nextframe>(); //下一帧执行 
        private Queue<Nextframe> m_removeQueue = new Queue<Nextframe>();

        public override void Clear()
        {
            base.Clear();

            m_nextframeMap.Clear();
            m_loadNodes.Clear();
        }

        protected LoadNode GetLoadNode(AssetLoadHandler handler)
        {
            if (m_loadNodes.ContainsKey(handler.id))
            {
                return m_loadNodes[handler.id];
            }
            return null;
        }

        //如果存在加载过的,直接下一帧返回,不需要启动协程
        protected override void OnStartLoad(AssetLoadHandler handler)
        {
            if (handler == null)
                return;

            if (!m_loadNodes.ContainsKey(handler.id))
            {
                handler.loader = this;
                var loadNode = GetOrCreateLoadNode();
                var loadMode = OnLoadMode(handler);

                if (loadMode == LoadMode.Coroutine)
                {
                    loadNode.coroutine = StartCoroutine(LoadAssetCoroutine(handler));
                }
                else if (loadMode == LoadMode.Nextframe)
                {
                    loadNode.nextframe = StartNextframe(() => { LoadAssetNextframe(handler); });
                }
                else if (loadMode == LoadMode.Immediately)
                {
                    OnLoadAssetSync(handler);
                }

                m_loadNodes[handler.id] = loadNode;
            }
        }

        protected override void OnStopLoad(AssetLoadHandler handler)
        {
            if (handler == null)
                return;

            if (m_loadNodes.ContainsKey(handler.id))
            {
                var loadNode = m_loadNodes[handler.id];
                if (loadNode.loadMode == LoadMode.Coroutine)
                {
                    StopCoroutine(loadNode.coroutine);
                }
                else if (loadNode.loadMode == LoadMode.Nextframe)
                {
                    StopNextframe(loadNode.nextframe);
                }

                m_loadNodes.Remove(handler.id);
            }
        }
        
        protected override void OnLoadSuccess(AssetLoadHandler handler)
        {
            base.OnLoadSuccess(handler);
            m_loadNodes.Remove(handler.id);
        }

        protected override void OnLoadFailed(AssetLoadHandler handler)
        {
            base.OnLoadFailed(handler);
            m_loadNodes.Remove(handler.id);
        }


        ////////////

        private IEnumerator LoadAssetCoroutine(AssetLoadHandler handler)
        {
            yield return OnLoadAssetAsync(handler);
        }

        private Nextframe StartNextframe(Action action)
        {
            var nextframe = new Nextframe();
            nextframe.action = action;
            m_nextframeMap.Add(nextframe);
            return nextframe;
        }

        private void StopNextframe(Nextframe nextFrame)
        {
            m_nextframeMap.Remove(nextFrame);
        }

        private void LoadAssetNextframe(AssetLoadHandler handler)
        {
            OnLoadAssetSync(handler);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            UpdateAction();
            UpdateRemove();
        }
        private void UpdateAction()
        {
            if (m_nextframeMap.Count > 0)
            {
                foreach (var nextframe in m_nextframeMap)
                {
                    if (!nextframe.isExecuted)
                    {
                        nextframe?.action();
                        nextframe.isExecuted = true;
                    }
                    else
                    {
                        m_removeQueue.Enqueue(nextframe);
                    }
                }
            }
        }

        private void UpdateRemove()
        {
            while(m_removeQueue.Count > 0)
            {
                var nextframe = m_removeQueue.Dequeue();
                m_nextframeMap.Remove(nextframe);
            }
        }

        private LoadNode GetOrCreateLoadNode()
        {
            return new LoadNode();
        }

        protected abstract LoadMode OnLoadMode(AssetLoadHandler handler);
        protected abstract IEnumerator OnLoadAssetAsync(AssetLoadHandler handler);
        protected abstract void OnLoadAssetSync(AssetLoadHandler handler);
    }
}

