using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary;

namespace ASGame
{
    /*
        包括公共的(即所有节点都相同
        私有的(节点不一定同
     * */
    //模型特效如果在UI上显示,需要设置层级
    public class ModelEffect : MonoBehaviour
    {
        public static readonly string MODFX_NAME = "fx_";
        [System.Serializable]
        public class NodeInfo
        {
            public GameObject nodeEffect;
            public string nodePath;
        }
        public class NodeMark : MonoBehaviour { }

        public List<NodeInfo> nodeInfos;
        private GameObject m_targetObj;
        private List<GameObject> m_trgetEffects;

#if UNITY_EDITOR
        public static void Make(GameObject modelWithEffect,GameObject modfxContainer)
        {
            if (modelWithEffect == null || modfxContainer == null)
                return ;

            //搜集所有含NodeMark或名字是_modfx的节点
            
            Stack<GameObject> parentNodes = new Stack<GameObject>();
            parentNodes.Push(modelWithEffect);

            var nodeList = new List<GameObject>();
            while (parentNodes.Count > 0)
            {
                var curNode = parentNodes.Pop();
                for (int i = 0; i < curNode.transform.childCount; i++)
                {
                    var itNode = curNode.transform.GetChild(i).gameObject;
                    if (curNode == itNode) continue;

                    var nodeMark = itNode.GetComponent<NodeMark>();
                    var nodeName = itNode.name;
                    if (nodeMark != null || nodeName.Contains(MODFX_NAME))
                    {
                        nodeList.Add(itNode);
                        
                        continue;
                    }
                    parentNodes.Push(itNode);
                    
                }
            }

            //
            Dictionary<GameObject, GameObject> nodeMap = new Dictionary<GameObject, GameObject>();
            var modfxComp = modfxContainer.GetComponent<ModelEffect>() ?? modfxContainer.AddComponent<ModelEffect>();
            modfxComp.nodeInfos = modfxComp.nodeInfos ?? new List<NodeInfo>();
            modfxComp.nodeInfos.Clear();

            foreach (var node in nodeList)
            {
                var nodeInfo = new NodeInfo();

                GameObject nodeEffect = Object.Instantiate(node, modfxContainer.transform, false);
                nodeMap[node] = nodeEffect; // 引用映射

                nodeEffect.name = nodeEffect.name.Replace("(Clone)","");
                string nodePath = XGameObjectTools.GetPathByGameObject(node);

                nodeInfo.nodeEffect = nodeEffect;
                nodeInfo.nodePath = nodePath;

                modfxComp.nodeInfos.Add(nodeInfo);
            }
            modfxContainer.name = modelWithEffect.name;

            //
            //尝试修复自定义节点的引用
            var particleSystems = modfxContainer.GetComponentsInChildren<ParticleSystem>();
            if (particleSystems != null && particleSystems.Length > 0)
            {
                foreach (var particleSystem in particleSystems)
                {
                    ParticleSystem.MainModule main = particleSystem.main;
                    if (main.simulationSpace == ParticleSystemSimulationSpace.Custom)
                    {
                        var oldRefGo = main.customSimulationSpace;
                        GameObject newRefGo = null;
                        if (oldRefGo != null)
                        {
                            if (nodeMap.TryGetValue(oldRefGo.gameObject, out newRefGo))
                            {
                                if (newRefGo != null)
                                {
                                    var newTrans = newRefGo.transform;
                                    main.customSimulationSpace = newTrans;
                                }
                            }
                        }
                    }
                }
            }
        }
#endif

        public GameObject GetTarget()
        {
            return m_targetObj;
        }

        public bool HadAttached()
        {
            return GetTarget() != null;
        }

        public void Attach(GameObject targetObj, bool isUseFullPath = true, bool isSetLayer = false)
        {
            if (targetObj == this)
                return;

            if (m_targetObj != null)
                Deattach();

            if (nodeInfos != null)
            {
                foreach(var nodeInfo in nodeInfos)
                {
                    var nodeEffect = nodeInfo.nodeEffect;
                    if (nodeEffect != null)
                    {
                        string nodeParentPath = Path.GetDirectoryName(nodeInfo.nodePath);
                        nodeParentPath = isUseFullPath ? nodeParentPath : Path.GetFileName(nodeParentPath);

                        Transform targetParentTrans = targetObj.transform.Find(nodeParentPath);
                        if (targetParentTrans != null)
                        {
                            m_trgetEffects = m_trgetEffects ?? new List<GameObject>();
                            nodeEffect.transform.SetParent(targetParentTrans, false);
                            nodeEffect.SetActive(true);

                            m_trgetEffects.Add(nodeEffect);

                            if (isSetLayer)
                            {
                                XGameObjectTools.SetLayer(nodeEffect, gameObject);
                            }
                        }
                        else
                        {
                            nodeEffect.SetActive(false);
                        }
                    }
                }
                m_targetObj = targetObj;
            }

        }

        public void Deattach()
        {
            if (m_targetObj == null)
                return;

            if (m_trgetEffects != null)
            {
                foreach (var nodeEffect in m_trgetEffects)
                {
                    if (nodeEffect != null)
                    {
                        nodeEffect.transform.SetParent(gameObject.transform, false);
                        nodeEffect.SetActive(false);
                    }
                }
                m_trgetEffects.Clear();
            }
        }

        public void Show(bool val)
        {
            if (m_trgetEffects != null && m_trgetEffects.Count > 0)
            {
                foreach (var nodeEffect in m_trgetEffects)
                {
                    if (nodeEffect != null)
                    {
                        nodeEffect.SetActive(val);
                    }
                }
            }
        }

        public List<string> GetExposeBones()
        {
            var exposeNames = new List<string>();

            if (nodeInfos != null && nodeInfos.Count > 0)
            {
                foreach (var nodeInfo in nodeInfos)
                {
                    string nodeParentPath = Path.GetDirectoryName(nodeInfo.nodePath);
                    string nodeParentName = Path.GetFileName(nodeParentPath);
                    exposeNames.Add(nodeParentName);
                }
            }

            return exposeNames;
        }

        ////
        private void OnDestroy()
        {
            Deattach();
        }

    }

}
