using UnityEditor;
using UnityEngine;
using XLibrary;

namespace XLibraryEditor
{
    public static class GUIUtil
    {
        //路径条
        public static void ShowPathBar(string text, ref string path, string desc = "Source Folder Path")
        {
            EditorGUILayout.BeginHorizontal();
            path = EditorGUILayout.TextField(text, path);
            if (GUILayout.Button("浏览"))
            {
                var selectedFolderPath = EditorUtility.SaveFolderPanel(desc, "Assets", "Sprites");
                if (selectedFolderPath != "")
                {
                    path = XFileTools.GetFileRelativePath(selectedFolderPath);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
