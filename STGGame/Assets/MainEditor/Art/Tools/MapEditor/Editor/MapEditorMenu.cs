using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XLibEditor;
using XLibrary;

namespace STGEditor
{
 
    public static class MapEditorMenu
    {
        public static readonly string editorName = "MapEditor.unity";
        public static readonly string saveDefaultName = "MapEditor.unity";

        [MenuItem("Assets/STGEditor/资源编辑器/地图编辑器/生成地图编辑模板")]
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

        ///
        static void CreateEditor(string assetPath)
        {
            string selectRootPath = PathUtil.GetFileRootPath(assetPath);
            
            // 处理开始
            var scene = EditorSceneManager.OpenScene(GetEditorPath());
            if (scene != null)
            {


                // 保存新场景
                string savePath = PathUtil.Combine(selectRootPath, string.Format(saveDefaultName));
                savePath = PathUtil.GetUniquePath(savePath);

                EditorSceneManager.SaveScene(scene, savePath);
            }
            else
            {
                Debug.LogWarning("编辑器路径错误");
            }

        }

        private static string GetEditorPath()
        {
            var path = EditorHelper.GetScriptPath(typeof(MapEditorMenu));
            var folder = Path.GetDirectoryName(path);
            return Path.Combine(folder, string.Format("{0}", editorName));

        }
    }

}
