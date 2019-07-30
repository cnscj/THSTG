using UnityEngine;
using UnityEditor;
using THGame;
namespace THEditor
{
    public class ResourceConfigWindow : EditorWindow
    {

        [MenuItem("THFramework/资源工具/资源配置")]
        static void ShowWindow()
        {
            ResourceConfigWindow myWindow = (ResourceConfigWindow)EditorWindow.GetWindow(typeof(ResourceConfigWindow), false, "ResourceConfig", false);//创建窗口
            myWindow.Show();//展示
        }


        void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            //标题
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("资源配置");

            //路径设置
            EditorGUILayout.Space();
            ShowEditorResList();

            //参数设置
            EditorGUILayout.Space();

            GUILayout.EndVertical();
        }

        void ShowEditorResList()
        {
            foreach (var infos in ResourceConfig.GetInstance().editorResList)
            {
                infos.resName = EditorGUILayout.TextField("资源名", infos.resName);
                ShowPathBar("资源路径:", ref infos.editorFolder);
                infos.isAutoProcess = EditorGUILayout.Toggle("是否自动预处理", infos.isAutoProcess);
                if (GUILayout.Button("移除"))
                {
                    ResourceConfig.GetInstance().editorResList.Remove(infos);
                    return;
                }
                EditorGUILayout.Space();

            }
            if (GUILayout.Button("...."))
            {
                ResourceConfig.ReourcesConfigInfos info = new ResourceConfig.ReourcesConfigInfos();
                var buildList = ResourceConfig.GetInstance().editorResList;
                buildList.Add(info);
            }
        }

        //路径条
        void ShowPathBar(string text,ref string path,string desc = "Select Folder Path")
        {
            EditorGUILayout.BeginHorizontal();
            path = EditorGUILayout.TextField(text, path);
            if (GUILayout.Button("浏览"))
            {
                var selectedFolderPath = EditorUtility.SaveFolderPanel(desc, "Assets", "Effects");
                if (selectedFolderPath != "")
                {
                    path = XFileTools.GetFileRelativePath(selectedFolderPath);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        void ShowToggle(string title,ref bool isOpen)
        {
            isOpen = EditorGUILayout.Toggle(title, isOpen);
        }

        private void OnDestroy()
        {
            EditorUtility.SetDirty(ResourceConfig.GetInstance());
            AssetDatabase.SaveAssets();
        }
    }
}
