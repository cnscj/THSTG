using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XLibrary;
using STGGame.Editor;
using ASEditor;

namespace STGEditor
{
    public static class EffectEditorMenu
    {
        public static readonly string editorName = "EffectEditor.unity";
        public static readonly string saveDefaultName = "EffectEditor.unity";

        public static readonly string nodeNameModelFx = "ModelFx";
        public static readonly string viewFxNodeName = "603";


        [MenuItem("Assets/STGEditor/资源编辑器/特效编辑器/生成特效编辑场景")]
        public static void MenuCreateEditor()
        {
            var selected = Selection.activeObject;
            string selectPath = AssetDatabase.GetAssetPath(selected);
            if (selectPath != "")
            {
                string srcSelectPath = selectPath;
                CreateEditor(srcSelectPath);
            }
            else
            {
                Debug.LogError("没有选中文件或文件夹");
            }
        }

        [MenuItem("Assets/STGEditor/资源编辑器/特效编辑器/创建节点特效模板")]
        public static void CreateModelEffectWorkspace()
        {
            Object[] objs = Selection.objects;
            foreach (var obj in objs)
            {
                GameObject prefab = obj as GameObject;
                if (!prefab)
                {
                    Debug.LogError("请选择模型prefab文件");
                    return;
                }

                GameObject modelFxNode = GameObject.Find(nodeNameModelFx);
                if (modelFxNode)
                {
                    string prefabPath = AssetDatabase.GetAssetPath(prefab);
                    string rootFolderName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(prefabPath));
                    GameObject go = Object.Instantiate(prefab, modelFxNode.transform, false);
                    //改下名字
                    go.name = string.Format("{0}{1}", viewFxNodeName, rootFolderName);

                    var editor = go.AddComponent<ViewEffectEditor>();
                    editor.srcPrefab = prefab;

                    EditorGUIUtility.PingObject(go);
                }
                else
                {
                    Debug.LogError("打开特效编辑场景后再进行该操作");
                }
            }
        }
        ///
        static void CreateEditor(string assetPath)
        {
            string selectRootPath = XPathTools.GetFileRootPath(assetPath);

            // 处理开始
            var scene = EditorSceneManager.OpenScene(GetEditorPath());
            if (scene != null)
            {
                // 保存新场景
                string savePath = XPathTools.Combine(selectRootPath, string.Format(saveDefaultName));
                savePath = XPathTools.GetUniquePath(savePath);


                //做工,弄一个主角



                EditorSceneManager.SaveScene(scene, savePath);
            }
            else
            {
                Debug.LogWarning("编辑器路径错误");
            }

        }

        private static string GetEditorPath()
        {
            var path = EditorHelper.GetScriptPath(typeof(EffectEditorMenu));
            var folder = Path.GetDirectoryName(path);
            return Path.Combine(folder, string.Format("{0}", editorName));

        }
    }

}
