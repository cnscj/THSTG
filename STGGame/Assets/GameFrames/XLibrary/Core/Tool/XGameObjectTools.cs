using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XLibrary
{
    public static class XGameObjectTools
    {
        /// <summary>
        /// 取得GameIbject的路径
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static string GetPathByGameObject(GameObject obj, GameObject root = null)
        {
            string path = obj.name;
            while (obj.transform.parent != null && obj.transform.parent.gameObject != root)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "/" + path;
            }
            return path;
        }

        /// <summary>
        /// 根据路径取得GO
        /// </summary>
        /// <param name="go"></param>
        /// <param name="path"></param>
        /// <param name="isExceptLeafNode"></param>
        /// <returns></returns>
        public static GameObject GetGameObjectByPath(GameObject go, string path, bool isExceptLeafNode = false)
        {
            string finalPath = (isExceptLeafNode) ? path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal)) : path;
            Transform retTransform = go.transform.Find(finalPath);
            if (retTransform != null)
            {
                return retTransform.gameObject;
            }
            return null;
        }

        /// <summary>
        /// 遍历所有可用节点
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        public static void Traverse(GameObject obj, Action<GameObject> action, bool includeinactive = false)
        {
            if (obj == null || action == null)
                return;

            foreach(var transform in obj.GetComponentsInChildren<Transform>(includeinactive))
            {
                action.Invoke(transform.gameObject);
            }
            
        }

        /// <summary>
        /// 深度遍历
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        public static void TraverseDFS(GameObject obj, Action<GameObject> action)
        {
            if (obj == null || action == null)
                return;

            Stack<GameObject> parentNodes = new Stack<GameObject>();
            parentNodes.Push(obj);

            while (parentNodes.Count > 0)
            {
                var curNode = parentNodes.Pop();
                for (int i = 0; i < curNode.transform.childCount; i++)
                {
                    var itNode = curNode.transform.GetChild(i).gameObject;
                    if (curNode == itNode) continue;

                    parentNodes.Push(itNode);
                }

                action.Invoke(curNode);
            }

        }

        /// <summary>
        /// 广度遍历
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        public static void TraverseBFS(GameObject obj, Action<GameObject> action)
        {
            if (obj == null || action == null)
                return;

            Queue<GameObject> parentNodes = new Queue<GameObject>();
            parentNodes.Enqueue(obj);

            while (parentNodes.Count > 0)
            {
                var curNode = parentNodes.Dequeue();
                for (int i = 0; i < curNode.transform.childCount; i++)
                {
                    var itNode = curNode.transform.GetChild(i).gameObject;
                    if (curNode == itNode) continue;

                    parentNodes.Enqueue(itNode);
                }

                action.Invoke(curNode);
            }

        }

        /// <summary>
        /// 用一个GO填充另一个GO
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="src"></param>
        /// <param name="cndtFunc">填充条件</param>
        public static void UnionGameObject(GameObject dest, GameObject src, bool isReplace = false, Action<GameObject> action = null, Func<GameObject, bool> cndtFunc = null)
        {
            if (dest == null || src == null)
                return;


            TraverseBFS(src, (curNode) =>
            {
                if (curNode != src)
                {
                    string curNodePath = GetPathByGameObject(curNode, src);
                    GameObject destNode = GetGameObjectByPath(dest, curNodePath);
                    if (destNode == null)   //目标没有这个节点
                    {
                        string destParentPath = Path.GetDirectoryName(curNodePath);
                        GameObject destParentNode = GetGameObjectByPath(dest, destParentPath);
                        if (destParentNode != null)
                        {
                            if (cndtFunc == null || (cndtFunc != null && cndtFunc(curNode)))
                            {
                                var newNode = UnityEngine.Object.Instantiate(curNode, destParentNode.transform, false);
                                newNode.name = curNode.name;
                                destNode = newNode;
                            }
                        }
                    }

                    action?.Invoke(destNode);
                }
            });
        }

        public static void MergeGameObject(GameObject dest, GameObject src, bool isReplace = false, Action<GameObject> action = null, Func<GameObject, bool> cndtFunc = null)
        {

        }
    }

}
