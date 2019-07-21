using System.Collections;
using System.Collections.Generic;
using System.IO;
using THEditor;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
namespace STGEditor
{
    public class PrefabBuildProcessMap : PostProcesser
    {
        public PrefabBuildProcessMap(string md5Folder, string exportFolder) : base(md5Folder, exportFolder)
        {
        }

        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Scene", new string[1] { PrefabBuildConfig.srcMaps });
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

            // 保存新场景
            string savePath = GetExportPath(Path.GetFileName(assetPath));
            EditorSceneManager.SaveScene(scene, savePath);
        }
    }
}
