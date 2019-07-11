using UnityEngine;
using UnityEditor;

namespace THEditor
{
    public class BundleBuildeConfigWindow : EditorWindow
    {

        [MenuItem("THFrameweok/打包工具/全局配置")]
        static void ShowWindow()
        {
            BundleBuildeConfigWindow myWindow = (BundleBuildeConfigWindow)EditorWindow.GetWindow(typeof(BundleBuildeConfigWindow), false, "ResourceConfig", false);//创建窗口
            myWindow.Show();//展示
        }


        void OnGUI()
        {
            GUILayout.BeginVertical();

            //标题
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("打包配置");

            GUILayout.EndVertical();
        }
    }
}
