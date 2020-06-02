using System;
using UnityEditor;
using UnityEngine;

namespace ASEditor
{
    [CustomPropertyDrawer(typeof(AssetSyncItem))]
    public class AssetSyncItemDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //创建一个属性包装器，用于将常规GUI控件与SerializedProperty一起使用
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                //绘制name
                EditorGUIUtility.labelWidth = 100;//设置属性名宽度 Name HP
                position.height = EditorGUIUtility.singleLineHeight;   //输入框高度，默认一行的高度

                SerializedProperty nameProperty = property.FindPropertyRelative("name");
                nameProperty.stringValue = EditorGUI.TextField(new Rect(position)
                {
                    y = position.y + 0 * EditorGUIUtility.singleLineHeight,
                    width = Math.Max(0, position.width),
                }, "资源名", nameProperty.stringValue);


                EditorGUIUtility.labelWidth = 100;
                SerializedProperty srcPathProperty = property.FindPropertyRelative("srcPath");
                srcPathProperty.stringValue = EditorGUI.TextField(new Rect(position)
                {
                    y = position.y + 1 * EditorGUIUtility.singleLineHeight,
                    width = Math.Max(0, position.width - 100),
                }, "资源路径", srcPathProperty.stringValue);
                GUILayout.BeginArea(new Rect(position)
                {
                    x = EditorGUIUtility.currentViewWidth - 105,
                    y = position.y + 1 * EditorGUIUtility.singleLineHeight + 60,
                    width = 100,
                });
                if (GUILayout.Button("浏览"))
                {
                    var selectedPath = EditorUtility.SaveFolderPanel("Source Folder Path", "Assets", "");
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        srcPathProperty.stringValue = selectedPath;
                    }
                }
                GUILayout.EndArea();


                EditorGUIUtility.labelWidth = 100;
                SerializedProperty realSearceProperty = property.FindPropertyRelative("realSearcePath");
                realSearceProperty.stringValue = EditorGUI.TextField(new Rect(position)
                {
                    y = position.y + 2 * EditorGUIUtility.singleLineHeight,
                    width = Math.Max(0, position.width - 100),
                }, "比对扫描路径", realSearceProperty.stringValue);
                GUILayout.BeginArea(new Rect(position)
                {
                    x = EditorGUIUtility.currentViewWidth - 105,
                    y = position.y + 2 * EditorGUIUtility.singleLineHeight + 60,
                    width = 100,
                });
                if (GUILayout.Button("浏览"))
                {
                    var selectedPath = EditorUtility.SaveFolderPanel("Destination Folder Path", "Assets", "");
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        realSearceProperty.stringValue = AssetSyncConfiger.GetInstance().GetCheckFolderRelativePath(selectedPath);
                    }
                }
                GUILayout.EndArea();

                EditorGUIUtility.labelWidth = 100;
                SerializedProperty realSyncProperty = property.FindPropertyRelative("realSyncProperty");
                realSyncProperty.stringValue = EditorGUI.TextField(new Rect(position)
                {
                    y = position.y + 3 * EditorGUIUtility.singleLineHeight,
                    width = Math.Max(0, position.width - 100),
                }, "同步目标路径", realSyncProperty.stringValue);
                GUILayout.BeginArea(new Rect(position)
                {
                    x = EditorGUIUtility.currentViewWidth - 105,
                    y = position.y + 2 * EditorGUIUtility.singleLineHeight + 60,
                    width = 100,
                });
                if (GUILayout.Button("浏览"))
                {
                    var selectedPath = EditorUtility.SaveFolderPanel("Destination Folder Path", "Assets", "");
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        realSyncProperty.stringValue = AssetSyncConfiger.GetInstance().GetCheckFolderRelativePath(selectedPath);
                    }
                }
                GUILayout.EndArea();


                SerializedProperty searchModeProperty = property.FindPropertyRelative("searchMode");
                searchModeProperty.enumValueIndex = (int)(AssetSyncItem.SearchMode)EditorGUI.EnumPopup(new Rect(position)
                {
                    y = position.y + 4 * EditorGUIUtility.singleLineHeight,
                    width = Math.Max(0, position.width),
                }, "扫描模式", (AssetSyncItem.SearchMode)searchModeProperty.enumValueIndex);


                SerializedProperty searchKeyProperty = property.FindPropertyRelative("searchMode");
                searchKeyProperty.enumValueIndex = (int)(AssetSyncItem.SearchKey)EditorGUI.EnumPopup(new Rect(position)
                {
                    y = position.y + 5 * EditorGUIUtility.singleLineHeight,
                    width = Math.Max(0, position.width),
                }, "扫描关键字", (AssetSyncItem.SearchKey)searchKeyProperty.enumValueIndex);
            }
        }
    }
}
