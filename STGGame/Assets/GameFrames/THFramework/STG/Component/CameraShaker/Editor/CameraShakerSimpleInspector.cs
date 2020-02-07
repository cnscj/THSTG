
using UnityEngine;
using UnityEditor;
using THGame;
using System.Collections.Generic;

namespace THEditor
{
    [CustomEditor(typeof(CameraShakerSimple))]
    public class CameraShakerSimpleInspector : Editor
    {
        private CameraShakerSimple m_editor;
        private List<KeyValuePair<string, SerializedProperty>> normalProps = new List<KeyValuePair<string, SerializedProperty>>();


        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            serializedObject.Update();//属性序列化

            ShowNormalProps();
            ShowShakeToggle();

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }

        void ShowNormalProps()
        {
            ShowPropertys(normalProps);
        }

        void ShowShakeToggle()
        {
            bool shake = EditorGUILayout.Toggle("震动", m_editor.shake);  //在Inspector面板上序列化一个对象，并关联demoTest.Score属性
            if (m_editor.shake != shake)
            {
                m_editor.shake = shake;
            }
            
        }


        void OnEnable()
        {
            m_editor = (CameraShakerSimple)target;
            Clear();

            AddPropertys(normalProps, "上下", "shakeUpDown");
            AddPropertys(normalProps, "左右", "shakeLeftRight");
            AddPropertys(normalProps, "摇头", "shakeHead");

            AddPropertys(normalProps, "周期", "shakePeriod");
            AddPropertys(normalProps, "次数", "shakeCount");
        }


        void AddPropertys(List<KeyValuePair<string, SerializedProperty>> list, string name, string property)
        {
            SerializedProperty prop = serializedObject.FindProperty(property);
            KeyValuePair<string, SerializedProperty> pair = new KeyValuePair<string, SerializedProperty>(name, prop);
            if (prop != null)
            {
                list.Add(pair);
            }
        }
        void ShowPropertys(List<KeyValuePair<string, SerializedProperty>> list)
        {
            foreach (var pair in list)
            {
                EditorGUILayout.PropertyField(pair.Value, new GUIContent(pair.Key), true);
            }
        }
        void Clear()
        {
            normalProps.Clear();
        }

    }
}
