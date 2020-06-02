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
    public class ModelEffect : MonoBehaviour
    {
        public static readonly string MODFX_NAME = "_modfx";
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

            var nodeDict = new List<GameObject>();
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
                        nodeDict.Add(itNode);
                        continue;
                    }
                    parentNodes.Push(itNode);
                }
            }

            //
            var modfxComp = modfxContainer.GetComponent<ModelEffect>() ?? modfxContainer.AddComponent<ModelEffect>();
            modfxComp.nodeInfos = modfxComp.nodeInfos ?? new List<NodeInfo>();
            modfxComp.nodeInfos.Clear();

            foreach (var node in nodeDict)
            {
                var nodeInfo = new NodeInfo();

                GameObject nodeEffect = Object.Instantiate(node, modfxContainer.transform, false);
                nodeEffect.name = nodeEffect.name.Replace("(Clone)","");
                string nodePath = XGameObjectTools.GetPathByGameObject(node);

                nodeInfo.nodeEffect = nodeEffect;
                nodeInfo.nodePath = nodePath;

                modfxComp.nodeInfos.Add(nodeInfo);
            }
            modfxContainer.name = modelWithEffect.name;
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

        public void Attach(GameObject targetObj)
        {
            if (targetObj == this)
                return;

            if (m_targetObj != null)
                Deattach();

            if (nodeInfos != null && nodeInfos.Count > 0)
            {
                foreach(var nodeInfo in nodeInfos)
                {
                    var nodeEffect = nodeInfo.nodeEffect;
                    if (nodeEffect != null)
                    {
                        string nodeParentPath = Path.GetDirectoryName(nodeInfo.nodePath);
                        Transform targetParentTrans = targetObj.transform.Find(nodeParentPath);
                        if (targetParentTrans != null)
                        {
                            m_trgetEffects = m_trgetEffects ?? new List<GameObject>();
                            nodeEffect.transform.SetParent(targetParentTrans, false);
                            nodeEffect.SetActive(true);

                            m_trgetEffects.Add(nodeEffect);
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

            if (m_trgetEffects != null && m_trgetEffects.Count > 0)
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

        public void Destroy()
        {
            Object.Destroy(this);
        }
        ////
        private void OnDestroy()
        {
            Deattach();
        }

    }

}
