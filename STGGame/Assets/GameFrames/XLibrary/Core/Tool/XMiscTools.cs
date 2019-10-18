/*************************
 * 
 * 其他一些混杂的工具类
 * 
 * 
 **************************/
using System.Collections.Generic;
using UnityEngine;
namespace XLibrary
{
    public static class XMiscTools
    {
        //遇到root节点时结束,并返回
        public static string GetGameObjectPath(GameObject go, GameObject root = null)
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
                }while (it != null);

                while (goNameStack.Count > 0)
                {
                    string goName = goNameStack.Pop();
                    path = path + goName + "/";
                }
                path = path.Substring(0, path.Length - 1);
            }
            return path;
        }


    }
}