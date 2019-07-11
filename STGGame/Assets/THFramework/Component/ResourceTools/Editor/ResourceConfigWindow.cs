using UnityEngine;
using UnityEditor;

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
            GUILayout.BeginVertical();

            //标题
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("资源配置");

            GUILayout.EndVertical();
        }
    }
}
