using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary;


namespace ASGame
{
    //XXX:每次优化都要遍历一遍所有节点,非常耗时,待优化
    public class BoneOptimize : MonoBehaviour
    {
        public string exposeBoneKey;                                //只要包含了这个key的都会暴露
        public List<string> exposeBoneList;                         //名字或路径

        private bool m_isOptimezed = false;
        private bool m_hasVisited = false;

        public void AddBones(string[] bones)
        {
            if (bones != null)
            {
                exposeBoneList = exposeBoneList ?? new List<string>();
                foreach (var bone in bones)
                {
                    exposeBoneList.Add(bone);
                }
            }
        }

        public void AddBone(string bone)
        {
            AddBones(new string[] { bone });
        }

        public bool IsOptimizable()
        {
            var animator = gameObject.GetComponentInChildren<Animator>();
            if (animator)
            {
                return animator.isOptimizable;
            }
            
            return false;
        }

        public bool IsOptimezed()
        {
            return m_isOptimezed;
        }

        [ContextMenu("Optimize")]
        public void Optimize(string[] bones = null)
        {
            AddBones(bones);
            m_isOptimezed = OptimizeTransformHierarchyByNames(gameObject, exposeBoneList != null ? exposeBoneList.ToArray() : null);
        }

        [ContextMenu("Deoptmize")]
        public void Deoptmize(bool isSaveChildren = false)
        {
            if (isSaveChildren)
            {
                foreach (var subOptmize in gameObject.GetComponentsInChildren<BoneOptimize>())
                {
                    subOptmize.m_hasVisited = false;
                }
            }
            m_isOptimezed = DeoptimizeTransformHierarchy(gameObject, isSaveChildren);
        }

        bool OptimizeTransformHierarchyByNames(GameObject go, string[] exposedTransforms)
        {
            //必须要有Animator,且挂点必须与Avatar一致(否则报错),节点不存在报C++错
            if (go)
            {
                var animator = go.GetComponentInChildren<Animator>();
                if (animator)
                {
                    List<string> exposeNodes = new List<string>();
                    Dictionary<string, bool> bonesMap = new Dictionary<string, bool>();
                    foreach (var node in go.GetComponentsInChildren<Transform>())
                    {
                        if (!bonesMap.ContainsKey(node.name))
                        {
                            bonesMap.Add(node.name, true);
                        }
                        if (!string.IsNullOrEmpty(exposeBoneKey))
                        {
                            if (node.name.Contains(exposeBoneKey))
                            {
                                exposeNodes.Add(node.name);
                            }
                        }
                    }
                    
                    foreach (var bonePath in exposedTransforms)
                    {
                        string boneName = Path.GetFileName(bonePath);
                        if (bonesMap.ContainsKey(boneName))
                        {
                            exposeNodes.Add(boneName);
                        }
                    }
                    AnimatorUtility.OptimizeTransformHierarchy(go, exposeNodes.ToArray());
                    return true;
                }
            }
            return false;
        }

        bool DeoptimizeTransformHierarchy(GameObject go, bool isSaveChildren = false)
        {
            if (go)
            {
                if (isSaveChildren)
                {
                    //还原之前,找到已经优化的挂点,临时取出保存
                    GameObject tmpNode = new GameObject();
                    tmpNode.SetActive(false);
                    if (go.transform.parent != null)
                    {
                        tmpNode.transform.SetParent(go.transform.parent, false);
                    }

                    List<KeyValuePair<GameObject, string>> tmpList = new List<KeyValuePair<GameObject, string>>();
                    foreach (var subOptmize in go.GetComponentsInChildren<BoneOptimize>())
                    {
                        if (subOptmize.gameObject == go)
                            continue;

                        if (subOptmize.m_hasVisited)
                            continue;

                        //递归搞下
                        subOptmize.m_isOptimezed = DeoptimizeTransformHierarchy(subOptmize.gameObject, isSaveChildren);

                        tmpList.Add(new KeyValuePair<GameObject, string>(subOptmize.gameObject, subOptmize.transform.parent.name));
                        subOptmize.gameObject.transform.SetParent(tmpNode.transform, false);

                        m_hasVisited = true;
                    }

                    //还原
                    AnimatorUtility.DeoptimizeTransformHierarchy(go);

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

                return true;
            }
            return false;
        }
    }
}

