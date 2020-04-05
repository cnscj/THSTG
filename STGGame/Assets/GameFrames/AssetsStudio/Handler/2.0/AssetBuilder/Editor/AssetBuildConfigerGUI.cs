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
        ReorderableList reorderableList;

        [MenuItem("AssetsStudio/资源打包配置2", false, 3)]
        static void ShowWnd()
        {
            ShowWindow("资源打包配置");
        }

        protected override void OnInit()
        {
            var cfgObj = AddObject(AssetBuildConfiger.GetInstance());
            var prop = AddProperty("buildItemList");

            reorderableList = new ReorderableList(cfgObj, prop, true, true, true, true);
            reorderableList.elementHeight = 30;//设置单个元素的高度

            //绘制单个元素
            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) => 
            {
                var element = prop.GetArrayElementAtIndex(index);
                rect.height -= 4;
                rect.y += 2;
                EditorGUI.PropertyField(rect, element);
            };

            //背景色
            reorderableList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) => 
            {
                GUI.backgroundColor = Color.yellow;
            };

            //头部
            reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "常规打包项");
        }

        protected override void OnShow()
        {
            GUILayout.BeginVertical();

            //标题
            GUILayout.Space(10);
            GUILayout.Label("打包配置",new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.MiddleCenter });

            AssetBuildConfiger.GetInstance().targetType = (AssetBuildConfiger.BuildPlatform)EditorGUILayout.EnumPopup("当前平台", AssetBuildConfiger.GetInstance().targetType);
            AssetBuildConfiger.GetInstance().exportFolder = EditorGUILayout.TextField("输出目录", AssetBuildConfiger.GetInstance().exportFolder);
            AssetBuildConfiger.GetInstance().bundleSuffix = EditorGUILayout.TextField("输出后缀", AssetBuildConfiger.GetInstance().bundleSuffix);
            reorderableList.DoLayoutList();

            GUILayout.EndVertical();
        }
    }
}
