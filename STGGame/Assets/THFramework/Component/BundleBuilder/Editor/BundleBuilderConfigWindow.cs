using UnityEngine;
using UnityEditor;

namespace THEditor
{
    public class BundleBuilderConfigWindow : EditorWindow
    {

        [MenuItem("THFramework/资源工具/打包配置")]
        static void ShowWindow()
        {
            BundleBuilderConfigWindow myWindow = (BundleBuilderConfigWindow)EditorWindow.GetWindow(typeof(BundleBuilderConfigWindow), false, "ResourceConfig", false);//创建窗口
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
