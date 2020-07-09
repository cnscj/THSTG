using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using XLibrary;
using System.Text.RegularExpressions;

namespace ASEditor
{
    public static class EditorHelper
    {
        /// <summary>
        /// 取得脚本所在路径
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static string GetScriptPath(ScriptableObject script)
        {
            MonoScript m_Script = MonoScript.FromScriptableObject(script);
            return AssetDatabase.GetAssetPath(m_Script);
        }

        public static string GetScriptPath(MonoBehaviour script)
        {
            MonoScript m_Script = MonoScript.FromMonoBehaviour(script);
            return AssetDatabase.GetAssetPath(m_Script);
        }

        public static string GetScriptPath(Type script)
        {
            return AssetDatabase.FindAssets("t:Script")
            .Where(v => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(v)) == script.Name)
            .Select(AssetDatabase.GUIDToAssetPath)
            .FirstOrDefault();
        }


        /// <summary>
        /// 获取所有选择路径
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllSelectionsPaths()
        {
            var selectedObjs = Selection.objects;
            List<string> pathList = new List<string>();
            if (selectedObjs != null && selectedObjs.Length > 0)
            {
                foreach (var selected in selectedObjs)
                {
                    string selectPath = AssetDatabase.GetAssetPath(selected);
                    pathList.Add(selectPath);
                }
            }
            return pathList.ToArray();
        }

        /// <summary>
        /// 从选择项中获取路径
        /// </summary>
        /// <param name="suffixs"></param>
        /// <param name="isTrans"></param>
        /// <returns></returns>
        public static string[] GetPathsBySelections(string[] suffixs = null, bool isTraverse = false)
        {
            var selectedObjs = Selection.objects;
            List<string> retPathList = new List<string>();
            if (selectedObjs != null && selectedObjs.Length > 0)
            {
                List<string> filePathList = new List<string>();
                foreach (var selected in selectedObjs)
                {
                    string selectPath = AssetDatabase.GetAssetPath(selected);
                    if (File.Exists(selectPath))            //如果是文件
                    {
                        filePathList.Add(selectPath);
                    }
                    else if(Directory.Exists(selectPath))   //如果是文件夹
                    {
                        XFolderTools.TraverseFiles(selectPath, (fullPath) =>
                        {
                            string realPath = XPathTools.GetRelativePath(fullPath);
                            filePathList.Add(realPath);
                        }, isTraverse);
                    }
                }

                //筛选
                HashSet<string> suffixSet = null;
                if (suffixs != null && suffixs.Length > 0)
                {
                    suffixSet = new HashSet<string>();
                    foreach(var suffix in suffixs)
                    {
                        string lowSuffix = suffix.ToLower();
                        if (!suffixSet.Contains(lowSuffix))
                            suffixSet.Add(lowSuffix);
                    }
                }


                foreach (var path in filePathList)
                {
                    if (suffixSet != null)
                    {
                        string fileExtName = Path.GetExtension(path).ToLower();
                        if (suffixSet.Contains(fileExtName))
                        {
                            retPathList.Add(path);
                        }
                    }
                    else
                    {
                        retPathList.Add(path);
                    }

                }
            }
            return retPathList.ToArray();
        }
    }
}

