using UnityEngine;
using UnityEditor;
using THGame;

namespace THEditor
{
    public class BundleBuilderConfigWindow : EditorWindow
    {

        [MenuItem("THFramework/资源工具/打包配置")]
        static void ShowWindow()
        {
            BundleBuilderConfigWindow myWindow = (BundleBuilderConfigWindow)EditorWindow.GetWindow(typeof(BundleBuilderConfigWindow), false, "BundleBuilderConfig", false);//创建窗口
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

            ShowBundleConfig();
            ShowListItem();


            GUILayout.EndVertical();
        }

        void ShowBundleConfig()
        {
            BundleBuilderConfig.GetInstance().targetType = (BundleBuilderConfig.BuildPlatform)EditorGUILayout.EnumPopup("当前平台", BundleBuilderConfig.GetInstance().targetType);
            BundleBuilderConfig.GetInstance().isBuildShare = EditorGUILayout.Toggle("公共部分单独打包", BundleBuilderConfig.GetInstance().isBuildShare);
            ShowPathBar("导出路径:", ref BundleBuilderConfig.GetInstance().exportFolder);
            EditorGUILayout.Space();
        }

        void ShowListItem()
        {
            foreach (var infos in BundleBuilderConfig.GetInstance().buildInfoList)
            {
                infos.srcName = EditorGUILayout.TextField("资源名", infos.srcName);
                ShowPathBar("资源路径:", ref infos.srcResFolder);
                infos.bundleName = EditorGUILayout.TextField("包名", infos.bundleName);
                if (GUILayout.Button("移除"))
                {
                    BundleBuilderConfig.GetInstance().buildInfoList.Remove(infos);
                    return;
                }
                EditorGUILayout.Space();

            }
            if (GUILayout.Button("...."))
            {
                BundleBuilderInfos info = new BundleBuilderInfos();
                var buildList = BundleBuilderConfig.GetInstance().buildInfoList;
                buildList.Add(info);
            }
        }

        //路径条
        void ShowPathBar(string text, ref string path, string desc = "Source Folder Path")
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

    }
}
