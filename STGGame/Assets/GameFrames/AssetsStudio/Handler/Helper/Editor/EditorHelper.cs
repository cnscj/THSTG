using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

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
    }
}

