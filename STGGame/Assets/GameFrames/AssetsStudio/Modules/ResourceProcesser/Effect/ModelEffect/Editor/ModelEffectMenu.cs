using UnityEngine;
using UnityEditor;
using XLibrary;
using ASGame;
using ASGame.Editor;

namespace ASEditor
{
    public class ModelEffectMenu
    {
        private const string s_uniquefilePrefix = "_modfx";
        private const string s_commonfilePrefix = "_modfx";
        private const int s_commonStartId = 500000001;
        private const string s_fileSuffix = "";
        private const string s_effectNodeName = "_fx";
        private const string s_exportUniquePath = "Assets/Effects/ModelEffectPrefabs";

        // [MenuItem("GameObject/Guangyv/模型特效菜单/模型特效", false, 0)]
        // public static void CreateModelEffect()
        // {
        //     GameObject newObject = new GameObject("GYModelEffect");
        //     newObject.AddComponent<ModelEffectEditor>();
        // }

        //[MenuItem("GameObject/Guangyv/其他/获取节点路径", false, 100)]
        public static void GetEffectNodePath()
        {
            if (Selection.activeGameObject != null)
            {
                string path = ModelEffectUtil.GetPathByGO(Selection.activeGameObject);
                Debug.Log(path);
                GUIUtility.systemCopyBuffer = path;
            }
        }

        ///
        [MenuItem("Assets/ASEditor/资源编辑菜单/创建模型特效模板", false, 0)]
        public static void CreateModelEffectWorkspace()
        {
            var selected = Selection.activeObject;
			string selectPath = AssetDatabase.GetAssetPath(selected);

            GameObject newPrefabGO = new GameObject();
            var editor = newPrefabGO.AddComponent<ModelEffectEditor>();
            editor.isCommonModelEffect = true;

            string saveFilePath = "";
            string fileId = XStringTools.SplitPathId(selectPath);
			if (string.IsNullOrEmpty(fileId))
			{
                // 公共特效
                int i = 0;
                do
                {
                    saveFilePath = selectPath + string.Format("/{0}{1}{2}.prefab",s_commonStartId+i,s_commonfilePrefix,s_fileSuffix);
                    i++;
                }while(XFileTools.Exists(saveFilePath));
			}
            else
            {   
                string uniquePath = s_exportUniquePath;
                if (!XFolderTools.Exists(uniquePath))
                {
                    uniquePath = selectPath;
                }
                //独有模型特效
                saveFilePath = uniquePath + string.Format("/{0}{1}{2}.prefab",fileId,s_uniquefilePrefix,s_fileSuffix);
                for (int i = 0; XFileTools.Exists(saveFilePath); i++)
                {
                    saveFilePath = uniquePath + string.Format("/{0}{1}{2}({3}).prefab",fileId,s_uniquefilePrefix,s_fileSuffix,i);
                }

                string modelPrefabPath = string.Format("{0}/{1}.prefab",selectPath,fileId);
                if (XFileTools.Exists(modelPrefabPath))
			    {
                    GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPrefabPath);
                    if (modelPrefab != null)
                    {
                        editor.modelPrefab = modelPrefab;
                        editor.Init();
                    }
                }
                
            }
			
            GameObject exportPrefab = PrefabUtility.SaveAsPrefabAsset(newPrefabGO, saveFilePath);
            GameObject.DestroyImmediate(newPrefabGO);

            EditorGUIUtility.PingObject(exportPrefab);  //跳转定位
        }
    }
}
