using System;
using THGame;
using THGame.Skill;
using UnityEditor;
using UnityEngine;

namespace THEditor
{
    [CustomEditor(typeof(SkillTriggerPlayableClip))]
    public class SkillTriggerPlayableClipInspector : Editor
    {
        SkillTriggerPlayableClip m_target;
        private SerializedProperty m_args;

        void OnEnable()
        {
            m_target = (SkillTriggerPlayableClip)target;
            m_type = serializedObject.FindProperty("type");
            m_args = serializedObject.FindProperty("args");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();//属性序列化

            var oriType = SkillTriggerType.Nop;
            if(!string.IsNullOrEmpty(m_target.type))
            {
                oriType = (SkillTriggerType)Enum.Parse(typeof(SkillTriggerType), m_target.type);
            }
            
            var triggerType = (SkillTriggerType)EditorGUILayout.EnumPopup("Type", oriType);
            var triggerStr = Enum.GetName(typeof(SkillTriggerType), triggerType);

            m_target.type = triggerStr;

            EditorGUILayout.PropertyField(m_args, new GUIContent("Args"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

