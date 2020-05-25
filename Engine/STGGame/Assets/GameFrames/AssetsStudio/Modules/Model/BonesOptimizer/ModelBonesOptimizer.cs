using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using XLibrary;

namespace ASGame
{
    public class ModelBonesOptimizer : MonoBehaviour
    {
        //节点标记类
        public class NodeMark: MonoBehaviour{}

        [SerializeField]
        public List<string> exposeBones;   //暴露的骨骼名字

        private bool m_isOptimezed;
        private HashSet<string> m_nodeNameDict;

        public bool IsOptimezed()
        {
            return m_isOptimezed;
        }

        public void Optimize(bool isEffectChildren = true)
        {
            if (!IsCanOptmizeOrDeoptmize())
                return;

            //取出节点内部第一层的优化器
            var nodeDict = TakeOutChildren();

            //节点暴露列表
            var exposeList = GetExposeBones(nodeDict);  //父节点暴露出来,免得被优化

            AnimatorUtility.OptimizeTransformHierarchy(gameObject, exposeList.ToArray());
            m_isOptimezed = true;

            //复原回相应的地方去
            var childOptimizerList = PutBackChildren(nodeDict);

            //递归去
            if (isEffectChildren)
            {
                foreach (var optimizer in childOptimizerList)
                {
                    optimizer.Optimize(isEffectChildren);
                }
            }
        }
        public void Deoptimize(bool isEffectChildren = true)
        {
            if (!IsCanOptmizeOrDeoptmize())
                return;

            //取出节点内部第一层的优化器
            var nodeDict = TakeOutChildren();

            AnimatorUtility.DeoptimizeTransformHierarchy(gameObject);
            m_isOptimezed = false;

            //复原回相应的地方去
            var childOptimizerList = PutBackChildren(nodeDict);

            if (isEffectChildren)
            {
                foreach(var optimizer in childOptimizerList)
                {
                    optimizer.Deoptimize(isEffectChildren);
                }
            }
        }

        /////
        
        private List<ModelBonesOptimizer> GetOptimizerNodeInChildren()
        {
            //采用深度递归
            Stack<GameObject> parentNodes = new Stack<GameObject>();
            parentNodes.Push(gameObject);

            var nodeDict = new List<ModelBonesOptimizer>();
            while (parentNodes.Count > 0)
            {
                var curNode = parentNodes.Pop();
                for (int i = 0; i < curNode.transform.childCount; i++)
                {
                    var itNode = curNode.transform.GetChild(i).gameObject;
                    if (curNode == itNode) continue;

                    var optimizer = itNode.GetComponent<ModelBonesOptimizer>();
                    if (optimizer != null)
                    {
                        nodeDict.Add(optimizer);
                        continue;
                    }

                    parentNodes.Push(itNode);
                }
            }
            return nodeDict;
        }

        //取出优化器所在节点
        private Dictionary<ModelBonesOptimizer, GameObject> TakeOutChildren()
        {
            Dictionary<ModelBonesOptimizer, GameObject> nodeDict = new Dictionary<ModelBonesOptimizer, GameObject>();
            var children = GetOptimizerNodeInChildren();
            var parentNode = transform.parent;
            foreach(var child in children)
            {
                var childParent = child.transform.parent;
                nodeDict.Add(child, childParent.gameObject);

                child.transform.SetParent(parentNode?.transform, false);
            }
            return nodeDict;
        }

        //返回去有2种情况,优化时返回去,和还原时放回去
        private List<ModelBonesOptimizer> PutBackChildren(Dictionary<ModelBonesOptimizer, GameObject> nodeDict)
        {
            List<ModelBonesOptimizer> nodeList = new List<ModelBonesOptimizer>();//有些节点可能在还原回去的时候失败了
            foreach(var dictPair in nodeDict)
            {
                var parentNode = dictPair.Value;
                if (parentNode != null)
                {
                    var node = dictPair.Key;
                    if (node != null)
                    {
                        node.transform.SetParent(parentNode.transform, false);
                        nodeList.Add(node);
                    }
                }
                else//放不回去,直接销毁
                {
                    var node = dictPair.Key;
                    if (node != null)
                    {
                        Object.Destroy(node);
                    }
                }
            }

            return nodeList;
        }

        private string[] GetExposeBones(Dictionary<ModelBonesOptimizer, GameObject> dict)
        {
            List<string> exNodes = null;
            if (dict != null && dict.Count > 0)
            {
                exNodes = new List<string>();
                foreach(var node in dict.Values)
                {
                    if (node != null)
                    {
                        exNodes.Add(node.name);
                    }
                }
            }
            return GetExposeBones(exNodes);
        }

        private string[] GetExposeBones(List<string> exNodes = null)
        {
            HashSet<string> exposeList = new HashSet<string>();
            
            if (exposeBones != null && exposeBones.Count > 0)
            {
                foreach (var nodeName in exposeBones)
                {
                    bool isCanAdd = true;
                    if (exposeList.Contains(nodeName))
                    {
                        isCanAdd = false;
                    }
#if UNITY_EDITOR
                    var allNodeSet = GetNodeNameDict();
                    if (allNodeSet.Contains(nodeName))
                    {
                        isCanAdd = false;
                    }
#endif
                    if (isCanAdd)
                    {
                        exposeList.Add(nodeName);
                    }
                }

            }

            if (exNodes != null && exNodes.Count > 0)
            {
                foreach (var nodeName in exNodes)
                {
                    bool isCanAdd = true;
                    if (exposeList.Contains(nodeName))
                    {
                        isCanAdd = false;
                    }
#if UNITY_EDITOR
                    var allNodeSet = GetNodeNameDict();
                    if (allNodeSet.Contains(nodeName))
                    {
                        isCanAdd = false;
                    }
#endif
                    if (isCanAdd)
                    {
                        exposeList.Add(nodeName);
                    }
                }
            }
            return exposeList.ToArray();
        }

        private bool IsCanOptmizeOrDeoptmize()
        {
            return GetComponent<Animator>() != null;
        }

        private HashSet<string> GetNodeNameDict()
        {
            if (m_nodeNameDict == null)
            {
                var childTransArray = GetComponentsInChildren<Transform>();
                if (childTransArray != null && childTransArray.Length > 0)
                {
                    m_nodeNameDict = new HashSet<string>();
                    foreach (var childNode in childTransArray)
                    {
                        var nodeName = childNode.name;
                        if (m_nodeNameDict.Contains(nodeName))
                        {
                            m_nodeNameDict.Add(nodeName);
                        }
                    }
                }
            }
            return m_nodeNameDict;
        }
    }
}
