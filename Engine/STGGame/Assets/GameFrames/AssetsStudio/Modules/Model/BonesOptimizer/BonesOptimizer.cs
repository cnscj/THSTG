using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary;

namespace ASGame
{
    public class ModelBonesOptimize : MonoBehaviour
    {
        [SerializeField]
        public List<string> exposeBoneList = new List<string>();   //可以是路径或名字
        private bool m_isOptimezed = false;
        private bool m_hasVisited = false;

        public void AddBones(List<string> bones, bool isUseName = true)
        {
            if (bones != null)
            {
                foreach (var bone in bones)
                {
                    string newBones = isUseName ? Path.GetFileName(bone) : bone;
                    exposeBoneList.Add(newBones);
                }
            }
        }

        public void AddBone(string bone, bool isUseName = true)
        {
            List<string> oneList = new List<string>();
            oneList.Add(bone);
            AddBones(oneList, isUseName);
        }
        public bool IsOptimizable(GameObject modelGO)
        {
            if (modelGO)
            {
                var animator = modelGO.GetComponentInChildren<Animator>();
                if (animator)
                {
                    return animator.isOptimizable;
                }
            }
            return false;
        }

        public bool IsOptimezed(GameObject modelGO)
        {
            modelGO = modelGO ? modelGO : gameObject;
            var boneOptimize = modelGO.GetComponent<ModelBonesOptimize>();
            if (boneOptimize != null)
            {
                return boneOptimize.m_isOptimezed;
            }
            return false;
        }

        public void Optimize(GameObject modelGO = null, bool isUseName = true, bool isNeedChild = true)
        {
            modelGO = modelGO ? modelGO : gameObject;
            //优化前,先把所有移到和model同级在去优化
            var myAnimator = modelGO.GetComponent<Animator>();
            List<KeyValuePair<string, GameObject>> childAminatorList = null;
            if (!isNeedChild && myAnimator != null)
            {
                childAminatorList = new List<KeyValuePair<string, GameObject>>();
                var myAnimatorParentNodeTrans = myAnimator.gameObject.transform.parent;
                var myAnimatorParentNode = myAnimatorParentNodeTrans != null ? myAnimatorParentNodeTrans.gameObject : null;
                var animators = modelGO.GetComponentsInChildren<Animator>(true);
                foreach (var animator in animators)
                {
                    if (myAnimator == animator) //自己的就算了,要子的
                        continue;

                    //记录父节点名字
                    var animatorNode = animator.gameObject;
                    exposeBoneList.Add(XGameObjectTools.GetPathByGameObject(modelGO, animatorNode));
                    
                    childAminatorList.Add(new KeyValuePair<string, GameObject>(animatorNode.name, animatorNode));

                    animatorNode.transform.SetParent(myAnimatorParentNode.transform, false);//
                }
            }

            if (isUseName)
            {
                OptimizeTransformHierarchyByNames(modelGO, exposeBoneList.ToArray());
            }
            else
            {
                OptimizeTransformHierarchyByPaths(modelGO, exposeBoneList.ToArray());
            }
            var boneOptimize = modelGO.GetComponent<ModelBonesOptimize>();
            if (boneOptimize != null)
            {
                boneOptimize.m_isOptimezed = true;
            }


            //还原
            if (!isNeedChild && myAnimator != null)
            {
                foreach (var info in childAminatorList)
                {
                    var childNodeName = info.Key;
                    var childNode = info.Value;
                    var childNodeParent = modelGO.transform.Find(childNodeName);
                    if (childNodeParent == null)
                    {
                        foreach (var childTransNode in modelGO.GetComponentsInChildren<Transform>())
                        {
                            var nodeName = childTransNode.name;
                            if (nodeName == childNodeName)
                            {
                                childNodeParent = childTransNode;
                                break;
                            }
                        }
                    }

                    if (childNodeParent != null)
                    {
                        if (info.Value != null)
                        {
                            childNode.transform.SetParent(childNodeParent.transform, false);
                           
                        }
                    }
                    else
                    {
                        if (info.Value != null)
                        {
                            Destroy(info.Value);
                        }
                    }
                }
            }
        }

        public void Deoptmize(GameObject modelGO = null, bool isSaveChildren = true)
        {
            modelGO = modelGO ? modelGO : gameObject;
            foreach (var subOptmize in modelGO.GetComponentsInChildren<ModelBonesOptimize>())
            {
                subOptmize.m_hasVisited = false;
            }
            DeoptimizeTransformHierarchy(modelGO, isSaveChildren);
            var boneOptimize = modelGO.GetComponent<ModelBonesOptimize>();
            if (boneOptimize != null)
            {
                boneOptimize.m_isOptimezed = false;
            }
        }

        //////
        void OptimizeTransformHierarchyByPaths(GameObject go, string[] exposedTransforms)
        {
            //必须要有Animator,且挂点必须与Avatar一致(否则报错),节点不存在报C++错
            if (go)
            {
                var animator = go.GetComponentInChildren<Animator>();
                if (animator)
                {
                    List<string> exposeNodes = new List<string>();
                    foreach (var path in exposedTransforms)
                    {
                        var hangNode = go.transform.Find(path);
                        if (hangNode)
                        {
                            exposeNodes.Add(path);
                        }
                    }
                    AnimatorUtility.OptimizeTransformHierarchy(go, exposeNodes.ToArray());

                }
            }
        }

        void OptimizeTransformHierarchyByNames(GameObject go, string[] exposedTransforms, bool isDefaultG = false)
        {
            //必须要有Animator,且挂点必须与Avatar一致(否则报错),节点不存在报C++错
            if (go)
            {
                var animator = go.GetComponentInChildren<Animator>();
                if (animator)
                {
                    List<string> exposeNodes = new List<string>();
                    Dictionary<string, bool> bonesMap = new Dictionary<string, bool>();

                    if (isDefaultG)
                    {
                        foreach (var node in go.GetComponentsInChildren<Transform>())
                        {
                            string nodeName = node.name;
                            if (!bonesMap.ContainsKey(nodeName))
                            {
                                bonesMap.Add(nodeName, true);
                                if (nodeName.ToLower().Contains("g_")) //这种格式的挂点,自动暴露
                                {
                                    exposeNodes.Add(nodeName);
                                }
                            }
                        }
                    }

                    else
                    {
#if UNITY_EDITOR
                        foreach (var node in go.GetComponentsInChildren<Transform>())
                        {
                            string nodeName = node.name;
                            if (!bonesMap.ContainsKey(nodeName))
                            {
                                bonesMap.Add(nodeName, true);
                            }
                        }
#else
                        if (exposedTransforms != null && exposedTransforms.Length > 0)
                        {
                            foreach (var nodeName in exposedTransforms)
                            {
                                if (!bonesMap.ContainsKey(nodeName))
                                {
                                    bonesMap.Add(nodeName, true);
                                }
                            }
                            //exposeNodes.AddRange(exposedTransforms);
                        }
                        
#endif
                    }

                    if (exposedTransforms != null && exposedTransforms.Length > 0)
                    {
                        foreach (var bname in exposedTransforms)
                        {
                            if (bonesMap.ContainsKey(bname))
                            {
                                exposeNodes.Add(bname);
                            }
                        }
                    }
                    AnimatorUtility.OptimizeTransformHierarchy(go, exposeNodes.ToArray());
                }
            }
        }

        //优化了就尽量不要在去还原了,
        void DeoptimizeTransformHierarchy(GameObject go, bool isSaveChildren = false)
        {
            if (go)
            {
                if (isSaveChildren)
                {
                    //还原之前,找到已经优化的挂点,临时取出保存
                    GameObject tmpNode = new GameObject();
                    tmpNode.SetActive(true);
                    if (go.transform.parent != null)
                    {
                        tmpNode.transform.SetParent(go.transform.parent, false);
                    }
                    List<KeyValuePair<GameObject, string>> tmpList = new List<KeyValuePair<GameObject, string>>();
                    foreach (var subOptmize in go.GetComponentsInChildren<ModelBonesOptimize>())
                    {
                        if (subOptmize.gameObject == go)
                            continue;

                        if (subOptmize.m_hasVisited)
                            continue;

                        //递归搞下
                        DeoptimizeTransformHierarchy(subOptmize.gameObject, isSaveChildren);

                        tmpList.Add(new KeyValuePair<GameObject, string>(subOptmize.gameObject, subOptmize.transform.parent.name));
                        subOptmize.gameObject.transform.SetParent(tmpNode.transform, false);

                        subOptmize.m_hasVisited = true;
                    }

                    //还原
                    AnimatorUtility.DeoptimizeTransformHierarchy(go);

                    //在塞回去
                    //因为根本不知道具体路径
                    Dictionary<string, string> bonesPaths = new Dictionary<string, string>();
                    if (tmpList.Count > 0)
                    {
                        foreach (var bone in go.GetComponentsInChildren<Transform>())
                        {
                            if (!bonesPaths.ContainsKey(bone.name))
                            {
                                string nodePath = XGameObjectTools.GetPathByGameObject(bone.gameObject, go);
                                bonesPaths.Add(bone.name, nodePath);
                            }
                        }

                    }

                    foreach (var retNode in tmpList)
                    {
                        //节点尽量不要重名
                        if (bonesPaths.ContainsKey(retNode.Value))
                        {
                            string nodePath = bonesPaths[retNode.Value];
                            GameObject parentNode = XGameObjectTools.GetGameObjectByPath(go, nodePath);
                            if (parentNode != null)
                            {
                                retNode.Key.transform.SetParent(parentNode.transform, false);
                            }
                        }
                    }

                    Destroy(tmpNode);
                }
                else
                {
                    AnimatorUtility.DeoptimizeTransformHierarchy(go);
                }

            }
        }
    }
}
