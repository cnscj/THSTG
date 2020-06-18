using System;
using System.Collections.Generic;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;
namespace ASGame
{
    public abstract class BaseLoadMethod : BaseLoader
    {
        protected class Nextframe
        {
            public bool isExecuted;
            public Action action;
        }

        protected enum LoadMethod
        {
            //同步
            Immediately,    //立刻(可能无法设置回调,待讨论

            //异步
            Coroutine,      //协程
            Nextframe,      //下一帧
        }

        protected class LoadNode
        {
            public LoadMethod loadMethod;
            public Coroutine coroutine;
            public Nextframe nextframe;

            public void Reset()
            {
                loadMethod = LoadMethod.Coroutine;
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
                var loadMethod = GetLoadMethod(handler);

                if (loadMethod == LoadMethod.Coroutine)
                {
                    loadNode.coroutine = StartCoroutine(LoadAssetCoroutine(handler));
                }
                else if (loadMethod == LoadMethod.Nextframe)
                {
                    loadNode.nextframe = StartNextframe(() => { LoadAssetNextframe(handler); });
                }
                else if (loadMethod == LoadMethod.Immediately)
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
                if (loadNode.loadMethod == LoadMethod.Coroutine)
                {
                    StopCoroutine(loadNode.coroutine);
                }
                else if (loadNode.loadMethod == LoadMethod.Nextframe)
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

        private LoadMethod GetLoadMethod(AssetLoadHandler handler)
        {
            if (handler.mode == AssetLoadMode.Sync)
            {
                return LoadMethod.Immediately;
            }
            return OnLoadMethod(handler);
        }

        protected virtual LoadMethod OnLoadMethod(AssetLoadHandler handler) { return LoadMethod.Coroutine; }
        protected abstract IEnumerator OnLoadAssetAsync(AssetLoadHandler handler);
        protected abstract void OnLoadAssetSync(AssetLoadHandler handler);
    }
}

