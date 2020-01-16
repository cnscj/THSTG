using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using XLibrary;

namespace ASGame
{
    public static class ModelEffectUtil
    {
        private const string s_effectNodeFlag = "_fx";

        public static void RefreshModelEffectInfo(GameObject model, ref List<ModelEffectMetadata> metadataList)
        {
            metadataList = GetEffectInfoList(model);
        }

        public static bool IsModelEffectNode(GameObject go, bool isFlagNameIs = false)
        {
            if (go != null)
            {
                if (isFlagNameIs)
                {
                    if (s_effectNodeFlag == go.name)
                    {

                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public static GameObject CopyGO(GameObject copy, GameObject newGO = null)
        {
            if (copy != null)
            {
                if (newGO == null)
                {
                    newGO = GameObject.Instantiate(copy);
                }

                newGO.transform.localPosition = copy.transform.localPosition;
                newGO.transform.localScale = copy.transform.localScale;
                newGO.transform.localRotation = copy.transform.localRotation;

                newGO.name = copy.name;

            }
            return newGO;
        }

        // 取得节点的路径,go是待遍历节点,root为根节点,遇此节点则返回
        public static string GetPathByGO(GameObject go, GameObject root = null)
        {
            string path = "";
            Stack<string> goNameStack = new Stack<string>();
            if (go != null)
            {
                //递归上去查找路径
                GameObject it = go;
                do
                {
                    goNameStack.Push(it.name);

                    if (it.transform && it.transform.parent)
                    {
                        it = it.transform.parent.gameObject;
                        if (it == root)
                        {
                            it = null;
                        }
                    }
                    else
                    {
                        it = null;
                    }
                } while (it != null);

                while (goNameStack.Count > 0)
                {
                    string goName = goNameStack.Pop();
                    path = path + goName + "/";
                }
                path = path.Substring(0, path.Length - 1);
            }

            return path;
        }

        public static GameObject GetGOByPath(GameObject go, string path, bool isExceptLeafNode = false)
        {
            string finalPath = (isExceptLeafNode) ? path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal)) : path;
            Transform transfrom = go.transform;
            Transform retTransform = transfrom.Find(finalPath);
            if (retTransform != null)
            {
                return retTransform.gameObject;
            }
            return null;
        }

        public static int GetSkeletonUID(GameObject modelGO)
        {
            if (modelGO)
            {
                var meshNode = modelGO.transform.Find("mesh");
                if (meshNode)
                {
                    var smrComp = meshNode.GetComponent<SkinnedMeshRenderer>();
                    if (smrComp)
                    {
                        var bones = smrComp.bones;
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (var bone in bones)
                        {
                            string path = XGameObjectTools.GetPathByGameObject(bone.gameObject, modelGO);
                            stringBuilder.Append(path);
                            stringBuilder.Append("\r\n");

                        }
                        return stringBuilder.ToString().GetHashCode();
                    }
                }
            }
            return -1;
        }

        public static string GetPrevPathByPath(string path)
        {
            int lastIndexOf_ = path.LastIndexOf('/');
            string newPath = (lastIndexOf_ == -1) ? path : path.Substring(0, lastIndexOf_);
            return newPath;

        }

        public static string GetNodeNameByPath(string path)
        {
            string nodeName = Path.GetFileNameWithoutExtension(path);
            return nodeName;
        }

        public static bool IsOptimized(GameObject go)
        {
            if (go)
            {
                var animator = go.GetComponent<Animator>();
                if (animator)
                {
                    return !animator.isOptimizable;
                }

            }
            return false;
        }

        /// 查找某节点下名为name的节点
        private static GameObject FindChildByName(GameObject go, string name)
        {
            foreach (Transform child in go.transform)
            {
                if (child.gameObject.name == name)
                {
                    return child.gameObject;
                }
            }
            return null;
        }

        private static void ForeachChildrenDFS(GameObject root, Action<GameObject, string> callback, GameObject node = null)
        {
            if (node == null)
            {
                node = root;
            }

            if (IsModelEffectNode(node, true))
            {
                string path = GetPathByGO(node, root);
                callback(node, path);
                return;
            }

            foreach (Transform child in node.transform)
            {
                ForeachChildrenDFS(root, callback, child.gameObject);
            }
        }

        private static List<ModelEffectMetadata> GetEffectInfoList(GameObject root)
        {
            if (root != null)
            {
                List<ModelEffectMetadata> infoList = new List<ModelEffectMetadata>();
                ForeachChildrenDFS(root, (goNode, goPath) =>
                {
                    ModelEffectMetadata tmpData = new ModelEffectMetadata
                    {
                        effectGO = goNode,
                        bonePath = goPath
                    };

                    infoList.Add(tmpData);
                });
                return infoList;
            }
            return null;
        }
    }
}
