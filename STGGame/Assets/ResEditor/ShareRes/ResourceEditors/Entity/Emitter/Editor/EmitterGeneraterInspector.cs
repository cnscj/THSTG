using System.Collections;
using System.Collections.Generic;
using STGGame;
using UnityEditor;

namespace STGEditor
{
    [CustomEditor(typeof(EmitterGenerater))]
    public class EmitterGeneraterInspector : Editor
    {
        private EmitterGenerater m_editor;
        private SerializedProperty m_propLevel;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            serializedObject.Update();
            //TODO:

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }

        void OnEnable()
        {
            m_editor = (EmitterGenerater)target;
            m_propLevel = serializedObject.FindProperty("level");

        }

    }

}
