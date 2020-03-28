using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ASEditor
{
    /// <summary>
    /// 解决项目中 一样的资源（名字或者路径不同）存在两份的问题  （多人做UI出现的问题， 或者美术没有管理好资源）
    /// 如果是要替换资源的话， 那就直接替换好了
    /// 
    /// 以上可以这么操作的基础是，你的Unity项目内的.prefab .Unity 都可以直接用文本开看到数据，而不是乱码（二进制）。这一步很关键，怎么设置呢？
    /// 打开项目Unity编辑器：Edit —-> Project Settings —-> Editor 这样就会调到你的Inspector面板的Editor Settings 
    /// 设置 Asset Serialization 的Mode类型为：Force Text(默认是Mixed); 这样你就能看到你的prefab文件引用了哪些贴图，字体，prefab 等资源了
    /// </summary>
    public class GUIDRefReplace : EditorWindow
    {
        private static GUIDRefReplace _window;
        private Object _sourceOld;
        private Object _sourceNew;
 
        private string _oldGuid;
        private string _newGuid;
 
        private bool isContainScene = true;
        private bool isContainPrefab = true;
        private bool isContainMat = true;
        private bool isContainAsset = false;
        private bool isContainPlayable = false;

        private List<string> withoutExtensions = new List<string>();
 
 
        [MenuItem("AssetsStudio/资源工具/GUID全局替换")]   // 菜单开启并点击的   处理
        public static void GUIDRefReplaceWin()
        {
 
            // true 表示不能停靠的
            _window = (GUIDRefReplace)EditorWindow.GetWindow(typeof(GUIDRefReplace), true, "引用替换");
            _window.Show();
 
        }
 
        void OnGUI()
        {
            // 要被替换的（需要移除的）
            GUILayout.Space(20);
 
 
            _sourceOld = EditorGUILayout.ObjectField("旧的资源", _sourceOld, typeof(Object), true);
            _sourceNew = EditorGUILayout.ObjectField("新的资源", _sourceNew, typeof(Object), true);
 
 
            // 在那些类型中查找（.unity\.prefab\.mat）
            GUILayout.Space(20);
            GUILayout.Label("要在哪些类型中查找替换：");
            EditorGUILayout.BeginHorizontal();
 
            isContainScene = GUILayout.Toggle(isContainScene, ".unity");
            isContainPrefab = GUILayout.Toggle(isContainPrefab, ".prefab");
            isContainMat = GUILayout.Toggle(isContainMat, ".mat");
            isContainAsset = GUILayout.Toggle(isContainAsset, ".asset");
            isContainPlayable = GUILayout.Toggle(isContainPlayable, ".playable");

            EditorGUILayout.EndHorizontal();
 
 
            GUILayout.Space(20);
            if (GUILayout.Button("开始替换!"))
            {
                if (EditorSettings.serializationMode != SerializationMode.ForceText)
                {
                    Debug.LogError("需要设置序列化模式为 SerializationMode.ForceText");
                    ShowNotification(new GUIContent("需要设置序列化模式为 SerializationMode.ForceText"));
                }
                else if (_sourceNew == null || _sourceOld == null)
                {
                    Debug.LogError("不能为空！");
                    ShowNotification(new GUIContent("不能为空！"));
                }
                else if (_sourceNew.GetType() != _sourceOld.GetType())
                {
                    Debug.LogError("两种资源类型不一致！");
                    ShowNotification(new GUIContent("两种资源类型不一致！"));
                }
                else if (!isContainScene && !isContainPrefab && !isContainMat && !isContainAsset && !isContainPlayable)
                {
                    Debug.LogError("要选择一种 查找替换的类型");
                    ShowNotification(new GUIContent("要选择一种 查找替换的类型"));
                }
                else   // 执行替换逻辑
                {
                    StartReplace();
                }
            }
        }
 
        private void StartReplace()
        {
            var path = AssetDatabase.GetAssetPath(_sourceOld);
 
            _oldGuid = AssetDatabase.AssetPathToGUID(path);
            _newGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_sourceNew));
 
            Debug.Log("oldGUID = " + _oldGuid + "  " + "_newGuid = " + _newGuid);
 
            withoutExtensions = new List<string>();
            if (isContainScene)
            {
                withoutExtensions.Add(".unity");
            }
            if (isContainPrefab)
            {
                withoutExtensions.Add(".prefab");
            }
            if (isContainMat)
            {
                withoutExtensions.Add(".mat");
            }
            if (isContainAsset)
            {
                withoutExtensions.Add(".asset");
            }
            if (isContainPlayable)
            {
                withoutExtensions.Add(".playable");
            }
 
            Find();
        }
 
        /// <summary>
        /// 查找  并   替换 
        /// </summary>
        private void Find()
        {
            if (withoutExtensions == null || withoutExtensions.Count == 0)
            {
                withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" ,".playable"};
            }
 
            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
            .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            int startIndex = 0;
 
            if (files == null || files.Length == 0)
            {
                Debug.Log("没有找到 筛选的引用");
                return;
            }
 
            EditorApplication.update = delegate ()
            {
                string file = files[startIndex];
 
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);
 
                var content = File.ReadAllText(file);
                if (Regex.IsMatch(content, _oldGuid))
                {
                    Debug.Log(file, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));
 
                    content = content.Replace(_oldGuid, _newGuid);
 
                    File.WriteAllText(file, content);
                }
                else
                {
                    Debug.Log(file);
                }
 
                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
 
                    AssetDatabase.Refresh();
                    Debug.Log("替换结束");
                }
 
            };
        }
 
        private string GetRelativeAssetsPath(string path)
        {
            return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
        }

        public static void ReplaceGUIDs(Dictionary<string,string> guidMap, string []files)
        {
            if (guidMap == null || guidMap.Count <= 0)
                return;

            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    var content = File.ReadAllText(file);
                    foreach(var guidPair in guidMap)
                    {
                        var oldGUID = guidPair.Key;
                        var newGUID = guidPair.Value;
                        if (oldGUID != newGUID)
                        {
                            if (Regex.IsMatch(content, oldGUID))
                            {
                                content = content.Replace(oldGUID, newGUID);
                            }
                        }
                    }
                    File.WriteAllText(file, content);
                }
                AssetDatabase.Refresh();
            }
        }
        public static void ReplaceGUIDs(Dictionary<string, string> guidMap, string path, string[] suffix = null)
        {
            suffix = (suffix != null) ? suffix : new string[] { ".prefab", ".unity", ".mat", ".asset", ".playable" };
            path = !string.IsNullOrEmpty(path) ? path : Application.dataPath;
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => suffix.Contains(Path.GetExtension(s).ToLower())).ToArray();
            ReplaceGUIDs(guidMap, files);
        }

        public static void ReplaceGUIDs(string oldGUID, string newGUID, string[] files)
        {
            ReplaceGUIDs(new Dictionary<string, string> { { oldGUID, newGUID } }, files);
        }

        public static void ReplaceGUIDs(string oldGUID, string newGUID, string path, string []suffix = null)
        {
            suffix = (suffix != null) ? suffix : new string[] { ".prefab", ".unity", ".mat", ".asset", ".playable" };
            path = !string.IsNullOrEmpty(path) ? path : Application.dataPath;

            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => suffix.Contains(Path.GetExtension(s).ToLower())).ToArray();
            ReplaceGUIDs(new Dictionary<string, string> { { oldGUID, newGUID } }, files);
        }


    }
}
