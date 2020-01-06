using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

namespace XLibEditor
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
            MonoScript m_Script = MonoScript.FromScriptableObject(script); //更新 使用UnityEditor API 
            string path = AssetDatabase.GetAssetPath(m_Script);
            return path;
        }

        public static string GetScriptPath(MonoBehaviour script)
        {
            MonoScript m_Script = MonoScript.FromMonoBehaviour(script); //更新 使用UnityEditor API 
            string path = AssetDatabase.GetAssetPath(m_Script);
            return path;
        }

        public static string GetScriptPath(Type script)
        {
            string path = AssetDatabase.FindAssets("t:Script")
            .Where(v => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(v)) == script.Name)
            .Select(AssetDatabase.GUIDToAssetPath)
            .FirstOrDefault();

            return path;
        }
    }
}

