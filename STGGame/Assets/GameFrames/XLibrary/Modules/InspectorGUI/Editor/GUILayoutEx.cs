using UnityEditor;
using UnityEngine;
using XLibrary;

namespace XLibEditor
{
    public static class GUILayoutEx
    {
        //路径条
        public static string ShowPathBar(string label, string text, string desc = "Source Folder Path")
        {
            EditorGUILayout.BeginHorizontal();
            string path = EditorGUILayout.TextField(label, text);
            if (GUILayout.Button("浏览"))
            {
                var selectedFolderPath = EditorUtility.SaveFolderPanel(desc, "Assets", "");
                if (selectedFolderPath != null)
                {
                    path = XFileTools.GetFileRelativePath(selectedFolderPath);
                }
            }

            EditorGUILayout.EndHorizontal();
            return path;
        }
    }
}
