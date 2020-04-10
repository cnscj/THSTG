using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XLibEditor;

namespace ASEditor
{
    public class AssetBuildConfigerGUI : WindowGUI<AssetBuildConfigerGUI>
    {
        private ReorderableList m_itemSortedList;

        [MenuItem("AssetsStudio/资源打包配置2", false, 3)]
        static void ShowWnd()
        {
            ShowWindow("资源打包配置");
        }

        protected override void OnInit()
        {
            var cfgObj = AddObject(AssetBuildConfiger.GetInstance());
            var prop = AddProperty("buildItemList");

            m_itemSortedList = new ReorderableList(cfgObj, prop, true, true, true, true);
            m_itemSortedList.elementHeight = 90;//设置单个元素的高度

            //绘制单个元素
            m_itemSortedList.drawElementCallback = (rect, index, isActive, isFocused) => 
            {
                var element = prop.GetArrayElementAtIndex(index);
                rect.height -= 4;
                rect.y += 2;
                EditorGUI.PropertyField(rect, element);
            };

            //背景色
            m_itemSortedList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) => 
            {
                GUI.backgroundColor = Color.yellow;
            };

            //头部
            m_itemSortedList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "常规打包项");
        }

        protected override void OnShow()
        {
            GUILayout.BeginVertical();

            //标题
            GUILayout.Space(10);
            GUILayout.Label("打包配置",new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.MiddleCenter });
            if (GUILayout.Button("打包")) { AssetBuilderManager.GetInstance().Build(); }

            AssetBuildConfiger.GetInstance().targetType = (AssetBuildConfiger.BuildPlatform)EditorGUILayout.EnumPopup("当前平台", AssetBuildConfiger.GetInstance().targetType);
            AssetBuildConfiger.GetInstance().exportFolder = GUILayoutEx.ShowPathBar("输出目录", AssetBuildConfiger.GetInstance().exportFolder);
            AssetBuildConfiger.GetInstance().bundleSuffix = EditorGUILayout.TextField("默认包后缀", AssetBuildConfiger.GetInstance().bundleSuffix);
            AssetBuildConfiger.GetInstance().shareBundleName = EditorGUILayout.TextField("公共包名", AssetBuildConfiger.GetInstance().shareBundleName);
            AssetBuildConfiger.GetInstance().isCombinePlatformName = EditorGUILayout.Toggle("输出目录拼接平台名称", AssetBuildConfiger.GetInstance().isCombinePlatformName);
            
            m_itemSortedList.DoLayoutList();
            EditorGUILayout.HelpBox("包路径说明\n" +
                "{assetRootPath}表示资源父路径" +
                "{assetNameNotEx}表示资源不带后缀名" +
                "{assetName}表示资源名" +
                "{assetKey}表示资源名中到_的部分" +
                "{assetFlatPath}表示资源扁平化路径" +
                "{buildName}表示构建器名", MessageType.Info);

            GUILayout.EndVertical();
        }
    }
}
