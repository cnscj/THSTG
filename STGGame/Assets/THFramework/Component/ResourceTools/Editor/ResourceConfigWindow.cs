using UnityEngine;
using UnityEditor;
using THGame;
namespace THEditor
{
    public class ResourceConfigWindow : EditorWindow
    {

        [MenuItem("THFrameweok/资源工具/全局配置")]
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
            ShowPathBar("源特效路径:", ref ResourceConfig.GetInstance().srcFolderEffect, "Source Effect Path");
            ShowPathBar("源精灵路径:", ref ResourceConfig.GetInstance().srcFolderSprite, "Source Sprite Path");
            ShowPathBar("源模型路径:", ref ResourceConfig.GetInstance().srcFolderModel, "Source Model Path");
            ShowPathBar("源关卡路径:", ref ResourceConfig.GetInstance().srcFolderLevel, "Source Level Path");

            //参数设置
            EditorGUILayout.Space();
            ShowToggle("自动处理模型源文件", ref ResourceConfig.GetInstance().isAutoGenModelPrefab);
            ShowToggle("自动处理精灵源文件", ref ResourceConfig.GetInstance().isAutoGenSpriteClip);

            GUILayout.EndVertical();
        }

        //路径条
        void ShowPathBar(string text,ref string path,string desc)
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
       

    }
}
