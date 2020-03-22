using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XLibrary;

namespace ASEditor
{
    public static class ResourceMenu
    {
        [MenuItem("Assets/ASEditor/辅助菜单/取得文件(夹)路径", false, 12)]
        public static void MenuGenSpriteOneKey()
        {
            var selected = Selection.activeObject;
            string selectPath = AssetDatabase.GetAssetPath(selected);
            if (selectPath != "")
            {
                Debug.Log(selectPath);
            }
            else
            {
                Debug.LogError("没有选中文件或文件夹");
            }
        }

        [MenuItem("Assets/ASEditor/辅助菜单/取得文件(夹)Md5", false, 13)]
        public static void MenuGetFileMd5()
        {
            var selected = Selection.activeObject;
            string selectPath = AssetDatabase.GetAssetPath(selected);
            if (selectPath != "")
            {
                string[] checkList = ResourceUtil.GetDependFiles(selectPath, new string[] { "cs" });
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var filePath in checkList)
                {
                    string code = XStringTools.FileToMd5(filePath);
                    stringBuilder.Append(filePath);
                    stringBuilder.AppendLine();
                    stringBuilder.Append(code);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                }
                Debug.Log(string.Format("{0}", stringBuilder.ToString()));
            }
            else
            {
                Debug.LogError("没有选中文件或文件夹");
            }
        }


        [MenuItem("Assets/ASEditor/辅助菜单/Prefab是否被引用")]
        private static void OnSearchForReferences()
        {
            //确保鼠标右键选择的是一个Prefab
            if (Selection.gameObjects.Length != 1)
            {
                return;
            }

            //遍历所有游戏场景
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    //打开场景
                    var curScene = EditorSceneManager.OpenScene(scene.path);
                    //获取场景中的所有游戏对象
                    GameObject[] gos = curScene.GetRootGameObjects();
                    foreach (GameObject go in gos)
                    {
                        //判断GameObject是否为一个Prefab的引用
                        if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
                        {
                            UnityEngine.Object parentObject = PrefabUtility.GetCorrespondingObjectFromSource(go);
                            string path = AssetDatabase.GetAssetPath(parentObject);
                            //判断GameObject的Prefab是否和右键选择的Prefab是同一路径。
                            if (path == AssetDatabase.GetAssetPath(Selection.activeGameObject))
                            {
                                //输出场景名，以及Prefab引用的路径
                                Debug.Log(scene.path + "  " + GetGameObjectPath(go));
                            }
                        }
                    }
                }
            }
        }
        public static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }
    }
}
