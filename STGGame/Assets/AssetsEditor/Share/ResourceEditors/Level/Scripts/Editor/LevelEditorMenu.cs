using System.IO;
using STGGame;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XLibrary;

namespace STGEditor
{
 
    public static class LevelEditorMenu
    {
        public static readonly string editorLevelFolder = PathUtil.Combine(ResourceEditorsConfig.editorFolder, string.Format("Level"));
        public static readonly string editorLevelScenePath = PathUtil.Combine(editorLevelFolder, string.Format("LevelEditor.unity"));
        public static readonly string saveDefaultName = "LevelEditor.unity";

        [MenuItem("Assets/STGEditor/资源编辑器/关卡编辑器/生成关卡编辑模板")]
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
                //
                foreach(var node in scene.GetRootGameObjects())
                {
                    if (node.GetComponent<LevelRootInfo>())
                    {
                        if (PrefabUtility.IsPartOfPrefabInstance(node))
                        {
                            PrefabUtility.UnpackPrefabInstance(node, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                            break;
                        }
                    }
                }

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
    }

}
