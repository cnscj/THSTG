using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ASGame;

namespace ASEditor
{
    [CustomEditor(typeof(EffectLevelNode))]
    public class EffectLevelNodeInspector : Editor
    {
        private EffectLevelNode m_editor;
        private SerializedProperty m_level;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            serializedObject.Update();//属性序列化

            EditorGUILayout.PropertyField(m_level, new GUIContent("Effect Level"), true);

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }


        void OnSceneGUI()
        {
            //得到test脚本的对象
            m_editor = (EffectLevelNode)target;

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
            m_editor = (EffectLevelNode)target;
            m_level = serializedObject.FindProperty("level");

        }
    }
}
