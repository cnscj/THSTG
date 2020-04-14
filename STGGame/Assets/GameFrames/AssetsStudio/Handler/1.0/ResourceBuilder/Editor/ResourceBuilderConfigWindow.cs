using UnityEngine;
using UnityEditor;
using ASGame;
using XLibrary;

namespace ASEditor
{
    public class ResourceBuilderConfigWindow : EditorWindow
    {

        [MenuItem("AssetsStudio/资源打包配置",false,3)]
        static void ShowWindow()
        {
            ResourceBuilderConfigWindow myWindow = (ResourceBuilderConfigWindow)EditorWindow.GetWindow(typeof(ResourceBuilderConfigWindow), false, "ResourceBuilderConfig", false);//创建窗口
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
            ResourceBuilderConfig.GetInstance().targetType = (ResourceBuilderConfig.BuildPlatform)EditorGUILayout.EnumPopup("当前平台", ResourceBuilderConfig.GetInstance().targetType);

            ResourceBuilderConfig.GetInstance().isBuildShare = EditorGUILayout.Toggle("公共部分单独打包", ResourceBuilderConfig.GetInstance().isBuildShare);
            ResourceBuilderConfig.GetInstance().isUseLower = EditorGUILayout.Toggle("使用全小写路径", ResourceBuilderConfig.GetInstance().isUseLower);
            ShowPathBar("导出路径:", ref ResourceBuilderConfig.GetInstance().exportFolder);
            ResourceBuilderConfig.GetInstance().isUsePlatformName = EditorGUILayout.Toggle("导出路径衔接平台名称", ResourceBuilderConfig.GetInstance().isUsePlatformName);
            ResourceBuilderConfig.GetInstance().isClearAfterBuilded = EditorGUILayout.Toggle("打包后自动清空包名", ResourceBuilderConfig.GetInstance().isClearAfterBuilded);
            ResourceBuilderConfig.GetInstance().bundleIsUseFullPath = EditorGUILayout.Toggle("只允许全路径加载资源", ResourceBuilderConfig.GetInstance().bundleIsUseFullPath);
            EditorGUILayout.Space();
        }

        void ShowListItem()
        {
            foreach (var infos in ResourceBuilderConfig.GetInstance().buildInfoList)
            {
                infos.srcName = EditorGUILayout.TextField("资源类型", infos.srcName);
                ShowPathBar("资源路径:", ref infos.srcResFolder);

                EditorGUILayout.BeginHorizontal();
                infos.srcBundleSuffix = EditorGUILayout.TextField("需要打包的文件后缀('|'分割)", infos.srcBundleSuffix);
                infos.isTraversal = EditorGUILayout.Toggle("遍历所有子目录", infos.isTraversal);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                infos.bundleName = EditorGUILayout.TextField("包名", infos.bundleName);
                infos.isUsePathName = EditorGUILayout.Toggle("中间路径扁平化", infos.isUsePathName);
                infos.isCommonPrefixion = EditorGUILayout.Toggle("只取前缀名", infos.isCommonPrefixion);
                EditorGUILayout.EndHorizontal();

                infos.shareBundleName = EditorGUILayout.TextField("共享包名({0}为资源名)", infos.shareBundleName);




                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("上移"))
                {
                    int index = ResourceBuilderConfig.GetInstance().buildInfoList.IndexOf(infos);
                    int newIndex = Mathf.Max(0, index - 1) ;
                    ResourceBuilderConfig.GetInstance().buildInfoList.Remove(infos);
                    ResourceBuilderConfig.GetInstance().buildInfoList.Insert(newIndex, infos);

                    return;
                }
                if (GUILayout.Button("下移"))
                {
                    int index = ResourceBuilderConfig.GetInstance().buildInfoList.IndexOf(infos);
                    int newIndex = Mathf.Min(ResourceBuilderConfig.GetInstance().buildInfoList.Count - 1, index + 1);
                    ResourceBuilderConfig.GetInstance().buildInfoList.Remove(infos);
                    ResourceBuilderConfig.GetInstance().buildInfoList.Insert(newIndex, infos);

                    return;
                }
                if (GUILayout.Button("移除"))
                {
                    ResourceBuilderConfig.GetInstance().buildInfoList.Remove(infos);
                    return;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

            }
            if (GUILayout.Button("...."))
            {
                ResourceBuilderInfos info = new ResourceBuilderInfos();
                var buildList = ResourceBuilderConfig.GetInstance().buildInfoList;
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
                    path = XPathTools.GetRelativePath(selectedFolderPath);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
        private void OnDestroy()
        {
            EditorUtility.SetDirty(ResourceBuilderConfig.GetInstance());
            AssetDatabase.SaveAssets();
        }

    }
}
