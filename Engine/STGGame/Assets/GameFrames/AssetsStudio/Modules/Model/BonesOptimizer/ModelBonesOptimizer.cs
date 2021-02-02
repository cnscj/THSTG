using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/*

1: Animator 组件和SkinnedMeshRenderer必须在同一个对象中。
2：OptimizeTransformHierarchy 接口的参数 exposedTransforms是 需要暴露的骨骼名称数组，并且暴露骨骼名称不能带空格，否则模型会出现显示问题（unity自己接口这样，我也没办法）
3：模型fbx设置Optimize选项必须勾选
4：对SkinnedMeshRenderer中参数bones进行赋值时，必须保证对应transform存在，因为优化接口会删掉所有对象，然后从新生成骨骼节点对象。如果骨骼对象下有挂点，需要对挂点进行保存，优化完成后再对挂点或者其他额外模型进行还原。
https://www.cnblogs.com/hengsoft/p/10283628.html

*/
namespace ASGame
{
    public class ModelBonesOptimizer : MonoBehaviour
    {
        //节点标记类
        public class NodeMark : MonoBehaviour{}

        [SerializeField]
        public List<string> exposeBones;   //暴露的骨骼名字

        private bool m_isOptimezed;
        private HashSet<string> m_nodeNameDict;

        public void AddExposeBones(List<string> bones)
        {
            if (bones != null && bones.Count > 0)
            {
                exposeBones = exposeBones ?? new List<string>();
                exposeBones.AddRange(bones);
            }
        }

        public void AddExposeBonesUnique(List<string> bones)
        {
            HashSet<string> uniqueSet = new HashSet<string>();
            if (exposeBones != null && exposeBones.Count > 0)
            {
                foreach (var bonePath in exposeBones)
                {
                    var boneName = Path.GetFileNameWithoutExtension(bonePath);
                    if (!uniqueSet.Contains(boneName))
                    {
                        uniqueSet.Add(boneName);
                    }
                }
            }

            if (bones != null && bones.Count > 0)
            {
                foreach (var bonePath in bones)
                {
                    var boneName = Path.GetFileNameWithoutExtension(bonePath);
                    if (!uniqueSet.Contains(boneName))
                    {
                        uniqueSet.Add(boneName);
                    }
                }
            }

            exposeBones = uniqueSet.ToList();
        }

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
            var animator = gameObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Update(0.001f);
            }

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
            var animator = gameObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Update(0.001f);
            }
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
            //var parentNode = transform.parent;
            foreach(var child in children)
            {
                var childParent = child.transform.parent;
                var chuldParentNode = childParent != null ? childParent.gameObject : null;
                if (chuldParentNode != null)
                {
                    var nodeMark = chuldParentNode.GetComponent<NodeMark>();
                    if (nodeMark == null)
                        chuldParentNode.AddComponent<NodeMark>();
                }
                nodeDict.Add(child, chuldParentNode);

                child.transform.SetParent(null, false);
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
                        var parentTrans = parentNode != null ? parentNode.transform : null;
                        node.transform.SetParent(parentTrans, false);
                        if (parentNode != null)
                        {
                            var nodeMark = parentNode.GetComponent<NodeMark>();
                            if (nodeMark != null)
                                Destroy(nodeMark);
                        }

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
                foreach (var nodePath in exposeBones)
                {
                    bool isCanAdd = true;
                    string nodeName = Path.GetFileNameWithoutExtension(nodePath);
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
                foreach (var nodePath in exNodes)
                {
                    bool isCanAdd = true;
                    string nodeName = Path.GetFileNameWithoutExtension(nodePath);
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
