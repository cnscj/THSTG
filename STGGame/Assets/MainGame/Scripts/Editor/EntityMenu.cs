using System.IO;
using UnityEditor;
using UnityEngine;

namespace STGEditor
{
    public class EntityMenu
    {
        [MenuItem("Assets/STGEditor/实体模板/生成玩家实体",true,1)]
        public static void MenuCreateEntityPrefab()
        {
            var selected = Selection.activeObject;
            string selectPath = AssetDatabase.GetAssetPath(selected);

            CreateEntityPrefab("Player", selectPath);
        }
       
        private static void CreateEntityPrefab(string saveName,string savePath)
        {
            GameObject newGO = new GameObject(saveName);

            GameObject localNode = new GameObject("Local");
            localNode.transform.SetParent(newGO.transform);

            GameObject modelNode = new GameObject("Model");
            modelNode.transform.SetParent(localNode.transform);

            string finalPath = Path.Combine(savePath, string.Format("{0}{1}", saveName, ".prefab"));
            PrefabUtility.SaveAsPrefabAsset(newGO, finalPath);
            UnityEngine.Object.DestroyImmediate(newGO);
        }

    }
}

