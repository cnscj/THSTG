using System.Collections;
using System.Collections.Generic;
using System.IO;
using THEditor;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using STGGame;

namespace STGEditor
{
    public class PrefabBuilderProcessLevel : ResourceProcesser
    {
        public PrefabBuilderProcessLevel(string md5Folder, string exportFolder) : base(md5Folder, exportFolder)
        {
        }

        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Scene", new string[1] { PrefabBuildConfig.srcLevels });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                filList.Add(assetPath);
            }
            return filList;
        }

        protected override void OnOnce(string assetPath)
        {
            // 处理开始
            var scene = EditorSceneManager.OpenScene(assetPath);

            //除Root下的,全部移除
            foreach(var node in scene.GetRootGameObjects())
            {
                if (node.name != "Root")
                {
                    if (PrefabUtility.IsPartOfPrefabInstance(node))
                    {
                        PrefabUtility.UnpackPrefabInstance(node, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                    }
                    Object.DestroyImmediate(node);
                }
            }

            // 保存新场景
            string savePath = GetExportPath(Path.GetFileName(assetPath));
            EditorSceneManager.SaveScene(scene, savePath);
        }
    }
}
