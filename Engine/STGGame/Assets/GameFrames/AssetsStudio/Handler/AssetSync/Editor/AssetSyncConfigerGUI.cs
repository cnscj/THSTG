using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XLibEditor;

namespace ASEditor
{
    public class AssetSyncConfigerGUI : WindowGUI<AssetSyncConfigerGUI>
    {
        private ReorderableList m_itemSortedList;
        [MenuItem("AssetsStudio/资源同步配置", false, 5)]
        static void ShowWnd()
        {
            ShowWindow("资源同步配置");
        }

        protected override void OnInit()
        {
            var cfgObj = AddObject(AssetSyncConfiger.GetInstance());
            var prop = AddProperty("syncItems");

            m_itemSortedList = new ReorderableList(cfgObj, prop, true, true, true, true);
            m_itemSortedList.elementHeight = 100;//设置单个元素的高度

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
            m_itemSortedList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "常规同步项");
        }

        protected override void OnShow()
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("同步")) { AssetSyncManager.GetInstance().Sync(); }

            AssetSyncConfiger.GetInstance().minVersion = EditorGUILayout.IntField("最小同步版本", AssetSyncConfiger.GetInstance().minVersion);
            AssetSyncConfiger.GetInstance().repositoryRootPath = GUILayoutEx.ShowPathBar("版本库路径", AssetSyncConfiger.GetInstance().repositoryRootPath);


            m_itemSortedList.DoLayoutList();

            GUILayout.EndVertical();
        }

    }
}
