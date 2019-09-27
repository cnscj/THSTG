using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ASGame;

namespace ASEditor
{
    [CustomEditor(typeof(EffectLevelEditMono))]
    public class EffectLevelEditMonoInspector : Editor
    {
        private EffectLevelEditMono m_editor;

        private List<KeyValuePair<string, SerializedProperty>> m_normalProps = new List<KeyValuePair<string, SerializedProperty>>();

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            serializedObject.Update();//属性序列化


            ShowNormalProp();

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }

        void ShowNormalProp()
        {
            ShowPropertys(m_normalProps);
        }



        void OnSceneGUI()
        {
            //得到test脚本的对象
            m_editor = (EffectLevelEditMono)target;

            //开始绘制GUI
            Handles.BeginGUI();

            //规定GUI显示区域
            GUILayout.BeginArea(new Rect(0, 100, 100, 100));

            GUIStyle titleStyle2 = new GUIStyle();
            titleStyle2.fontSize = 20;
            titleStyle2.normal.textColor = new Color(46f / 256f, 163f / 256f, 256f / 256f, 256f / 256f);
            GUILayout.Label(string.Format("特效等级:{0}", m_editor.level), titleStyle2);


            GUILayout.EndArea();
            Handles.EndGUI();
        }

        void OnEnable()
        {
            m_editor = (EffectLevelEditMono)target;
            Clear();
            //AddPropertys(m_normalProps, "特效等级", "level");

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
            m_normalProps.Clear();
        }
    }
}
