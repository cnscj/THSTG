using System.Collections;
using System.Collections.Generic;
using System.IO;
using THGame;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace STGEditor
{
    public static class UIEditorMenu
    {
        public static readonly string editorUIFolder = PathUtil.Combine(ResourceEditorsConfig.editorFolder, string.Format("UI"));
        public static readonly string editorUIScenePath = PathUtil.Combine(editorUIFolder, string.Format("UIEditor.unity"));
        public static readonly string saveDefaultName = "UIEditor.unity";

        [MenuItem("Assets/STGEditor/资源编辑器/UI编辑器/生成UI编辑模板")]
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
            string selectFileName = Path.GetFileNameWithoutExtension(assetPath);

            // 处理开始
            var scene = EditorSceneManager.OpenScene(editorUIScenePath);
            if (scene != null)
            {
                // 保存新场景
                string savePath = PathUtil.Combine(selectRootPath, string.Format(saveDefaultName));
                savePath = PathUtil.GetUniquePath(savePath);


                //做工,弄一个主角



                EditorSceneManager.SaveScene(scene, savePath);
            }
            else
            {
                Debug.LogWarning("编辑器路径错误");
            }

        }
    }

}
