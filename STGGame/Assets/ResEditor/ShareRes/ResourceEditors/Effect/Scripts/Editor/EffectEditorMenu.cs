using System.Collections;
using System.Collections.Generic;
using System.IO;
using THGame;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace STGEditor
{
    public static class EffectEditorMenu
    {
        public static readonly string editorLevelFolder = PathUtil.Combine(ResourceEditorsConfig.editorFolder, string.Format("Effect"));
        public static readonly string editorLevelScenePath = PathUtil.Combine(editorLevelFolder, string.Format("EffectEditor.unity"));
        public static readonly string saveDefaultName = "EffectEditor.unity";

        [MenuItem("Assets/STGEditor/特效编辑/生成特效编辑模板")]
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
            var scene = EditorSceneManager.OpenScene(editorLevelScenePath);
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
