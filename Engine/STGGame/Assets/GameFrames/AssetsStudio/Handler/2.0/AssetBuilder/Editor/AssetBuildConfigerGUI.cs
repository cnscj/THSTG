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
        private Vector3 m_scrollPos = Vector2.zero;
        [MenuItem("AssetsStudio/资源打包配置", false, 3)]
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
            m_scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), m_scrollPos, new Rect(0, 0, position.width, 1000));
            //标题
            GUILayout.Space(10);
            GUILayout.Label("打包配置",new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.MiddleCenter });
            if (GUILayout.Button("打包")) { AssetBuilderManager.GetInstance().Build(); }

            AssetBuildConfiger.GetInstance().targetType = (AssetBuildConfiger.BuildPlatform)EditorGUILayout.EnumPopup("当前平台", AssetBuildConfiger.GetInstance().targetType);
            AssetBuildConfiger.GetInstance().exportFolder = GUILayoutEx.ShowPathBar("输出目录", AssetBuildConfiger.GetInstance().exportFolder);
            AssetBuildConfiger.GetInstance().shareBundleName = EditorGUILayout.TextField("公共包名", AssetBuildConfiger.GetInstance().shareBundleName);
            AssetBuildConfiger.GetInstance().isCombinePlatformName = EditorGUILayout.Toggle("输出目录拼接平台名称", AssetBuildConfiger.GetInstance().isCombinePlatformName);
            AssetBuildConfiger.GetInstance().isRidofSpecialChar = EditorGUILayout.Toggle("移除特殊字符", AssetBuildConfiger.GetInstance().isRidofSpecialChar);

            m_itemSortedList.DoLayoutList();
            EditorGUILayout.HelpBox("包路径说明\n" +
                "{assetEx}资源后缀" +
                "{assetRootPath}资源父路径" +
                "{assetNameNotEx}资源不带后缀名" +
                "{assetName}资源名" +
                "{assetKey}资源名中到_的部分" +
                "{assetFlatPath}资源扁平化路径" +
                "{buildName}构建器名", MessageType.Info);

            GUI.EndScrollView();
            GUILayout.EndVertical();
        }
    }
}
