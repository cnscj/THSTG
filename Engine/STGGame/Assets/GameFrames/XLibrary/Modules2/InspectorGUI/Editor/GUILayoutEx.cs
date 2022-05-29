using System;
using UnityEditor;
using UnityEngine;
using XLibrary;

namespace XLibEditor
{
    public static class GUILayoutEx
    {
        //路径条
        public static string ShowPathBar(string label, string text, string desc = "Source Folder Path", Func<string, string> callback = null)
        {
            EditorGUILayout.BeginHorizontal();
            string path = EditorGUILayout.TextField(label, text);
            if (GUILayout.Button("浏览"))
            {
                var selectedFolderPath = EditorUtility.SaveFolderPanel(desc, "Assets", "");
                if (!string.IsNullOrEmpty(selectedFolderPath))
                {
                    path = XPathTools.GetRelativePath(selectedFolderPath);
                    if (callback != null)
                    {
                        path = callback.Invoke(selectedFolderPath);
                    }
                    
                }
            }

            EditorGUILayout.EndHorizontal();
            return path;
        }
    }
}
