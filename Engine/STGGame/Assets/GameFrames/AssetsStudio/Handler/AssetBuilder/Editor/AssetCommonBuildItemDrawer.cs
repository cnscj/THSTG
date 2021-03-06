﻿using System;
using UnityEditor;
using UnityEngine;

namespace ASEditor
{
    [CustomPropertyDrawer(typeof(AssetCommonBuildItem))]
    public class AssetCommonBuildItemDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //创建一个属性包装器，用于将常规GUI控件与SerializedProperty一起使用
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                //绘制name
                EditorGUIUtility.labelWidth = 100;//设置属性名宽度 Name HP
                position.height = EditorGUIUtility.singleLineHeight;   //输入框高度，默认一行的高度

                SerializedProperty nameProperty = property.FindPropertyRelative("builderName");
                nameProperty.stringValue = EditorGUI.TextField(new Rect(position)
                {
                    y = position.y + 0 * EditorGUIUtility.singleLineHeight,
                    width = Math.Max(0, position.width - 60),
                },"资源名", nameProperty.stringValue);

                EditorGUIUtility.labelWidth = 30;
                SerializedProperty enabledProperty = property.FindPropertyRelative("isEnabled");
                enabledProperty.boolValue = EditorGUI.Toggle(new Rect(position)
                {
                    x = EditorGUIUtility.currentViewWidth - 60,
                    y = position.y + 0 * EditorGUIUtility.singleLineHeight,
                }, "启用", enabledProperty.boolValue);

                EditorGUIUtility.labelWidth = 100;
                SerializedProperty srcPathProperty = property.FindPropertyRelative("buildSrcPath");
                srcPathProperty.stringValue = EditorGUI.TextField(new Rect(position)
                {
                    y = position.y + 1 * EditorGUIUtility.singleLineHeight,
                    width = Math.Max(0, position.width - 100),
                }, "资源路径", srcPathProperty.stringValue);
                GUILayout.BeginArea(new Rect(position) {
                    x = EditorGUIUtility.currentViewWidth - 105,
                    y = position.y + 1 * EditorGUIUtility.singleLineHeight + 174+ 2*17,
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

                SerializedProperty regexFileNameProperty = property.FindPropertyRelative("regexFileName");
                regexFileNameProperty.stringValue = EditorGUI.TextField(new Rect(position)
                {
                    y = position.y + 2 * EditorGUIUtility.singleLineHeight
                },"被打包资源名(限正则)", regexFileNameProperty.stringValue);

                SerializedProperty assetBundleFormatNameProperty = property.FindPropertyRelative("assetBundleNameFormat");
                assetBundleFormatNameProperty.stringValue = EditorGUI.TextField(new Rect(position)
                {
                    y = position.y + 3 * EditorGUIUtility.singleLineHeight
                }, "资源包名(含格式化)", assetBundleFormatNameProperty.stringValue);

                SerializedProperty shareBundleNameFormat = property.FindPropertyRelative("shareBundleNameFormat");
                shareBundleNameFormat.stringValue = EditorGUI.TextField(new Rect(position)
                {
                    y = position.y + 4 * EditorGUIUtility.singleLineHeight
                }, "公共包名(含格式化)", shareBundleNameFormat.stringValue);
            }
        }
    }
}
