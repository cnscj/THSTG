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
            m_args = serializedObject.FindProperty("args");
        }

        public override void OnInspectorGUI()
        {
            if (!m_target)
                return;

            serializedObject.Update();//属性序列化

            m_target.type = EnumPopupStr(m_target.type);
            m_target.args = PropertyStringList(m_target.args);

            serializedObject.ApplyModifiedProperties();
        }

        private string EnumPopupStr(string oriTypeStr)
        {
            var oriType = SkillTriggerType.Nop;
            if (!string.IsNullOrEmpty(oriTypeStr))
            {
                oriType = (SkillTriggerType)Enum.Parse(typeof(SkillTriggerType), oriTypeStr);
            }

            var triggerType = (SkillTriggerType)EditorGUILayout.EnumPopup("Type", oriType);
            var triggerStr = Enum.GetName(typeof(SkillTriggerType), triggerType);

            return triggerStr;
        }

        private string[] PropertyStringList(string[] argsList)
        {
            //这里应该数据驱动,否则默认使用Args数组,如有特殊说明用自定义
            EditorGUILayout.PropertyField(m_args, new GUIContent("Args"), true);

            return argsList;
        }
    }
}

