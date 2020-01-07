using System.IO;
using XLibEditor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XLibrary;

namespace STGEditor
{
    public static class EffectEditorMenu
    {
        public static readonly string editorName = "EffectEditor.unity";
        public static readonly string saveDefaultName = "EffectEditor.unity";

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
