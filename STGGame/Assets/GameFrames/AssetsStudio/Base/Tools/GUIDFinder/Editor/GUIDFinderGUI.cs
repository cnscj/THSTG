 
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using XLibrary;

namespace ASGame
{
    public class GUIDFinderGUI : EditorWindow
    {
        private static bool m_isUpdate = true;
        private static Vector2 m_scrollPosition;
        private static Dictionary<string, ArrayList> m_gridDict = new Dictionary<string, ArrayList>();
        private static string m_selectedAssetPath;

        void OnGUI()
        {
            if (GUILayout.Button("Refresh"))
            {
                m_isUpdate = true;
            }

            if (m_isUpdate)
            {
                UpdateReferences();
                m_isUpdate = false;
            }

            if (m_selectedAssetPath == null)
            {
                m_selectedAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            }
    
            if (m_gridDict.ContainsKey(m_selectedAssetPath))
            {
                m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.Width(700), GUILayout.Height(500));
                foreach (string filePath in m_gridDict[m_selectedAssetPath])
                {
                    if (GUILayout.Button(filePath))
                    {
                        OpenFolderAndSelectFile(filePath);
                    }
                }
                GUILayout.EndScrollView();
            }
        }

        private static void OpenFolderAndSelectFile(string relativePath)
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(relativePath);
        }

        private static void UpdateReferences()
        {
            m_gridDict.Clear();
            string assetDir = Application.dataPath;

            var withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset", ".controller", ".playable" };
            string[] files = Directory.GetFiles(assetDir, "*.*", SearchOption.AllDirectories)
                    .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower()))
                    .ToArray();

            int startIndex = 0;
            EditorApplication.update = delegate ()
            {
                string file = files[startIndex];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);

                string relativePath = XFileTools.GetFileRelativePath(file);
                string[] dps = AssetDatabase.GetDependencies(relativePath);
                foreach (string path in dps)
                {
                    if (!m_gridDict.ContainsKey(path))
                    {
                        m_gridDict.Add(path, new ArrayList());
                    }
                    ArrayList existFiles = m_gridDict[path];
                    existFiles.Add(relativePath);
                }

                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                }
            };
        }

        [MenuItem("Assets/Find References In Project", true)]
        private static bool CheckValidation()
        {
            return Selection.activeObject != null;
        }

        [MenuItem("Assets/Find References In Project")]
        public static void ShowWindow()
        {
            m_selectedAssetPath = null;
            EditorWindow.GetWindow(typeof(GUIDFinderGUI));
        }

        [MenuItem("Assets/Find Dependencies To Console")]
        public static void FindDependencies()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string deps = path + " deps:";
            foreach (var dep in AssetDatabase.GetDependencies(path))
            {
                deps += "\n" + dep;
            }
            Debug.Log(deps);            
        }

    }
}
