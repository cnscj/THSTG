using System.Collections;
using System.Collections.Generic;
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

                SerializedProperty nameProperty = property.FindPropertyRelative("builderName");//找到每个属性的序列化值
                nameProperty.stringValue = EditorGUI.TextField(position, nameProperty.displayName, nameProperty.stringValue);


            }
        }
    }
}
