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
        string[] m_argsDesc;

        void OnEnable()
        {
            m_target = (SkillTriggerPlayableClip)target;
        }

        public override void OnInspectorGUI()
        {
            if (!m_target)
                return;

            serializedObject.Update();//属性序列化


            var oriTypeStr = m_target.type;
            var argsDesc = m_argsDesc;

            m_target.type = EnumPopupStr(oriTypeStr);


            if (string.Compare(m_target.type, oriTypeStr) != 0)
            {
                m_argsDesc = GetDescList();
                argsDesc = m_argsDesc;
            }

            m_target.args = PropertyStringList(m_target.args, argsDesc);

            serializedObject.ApplyModifiedProperties();
        }

        private string[] GetDescList()
        {
            //:貌似不是很友好
            string[] descList = null;
            //根据type实例化一个clip,然后获取相应数据
            var triggerFactor = SkillTriggerManager.GetInstance().GetFactory(m_target.type);
            if (triggerFactor != null)
            {
                var trigger = triggerFactor.CreateTrigger();
                if (trigger != null)
                {
                    descList = trigger.OnArgsDesc();
                }
                triggerFactor.RecycleTrigger(trigger);
            }
            return descList;
        }

        private string EnumPopupStr(string oriTypeStr)
        {
            var oriType = SkillTriggerType.Nop;
            if (!string.IsNullOrEmpty(oriTypeStr))
            {
                 Enum.TryParse(oriTypeStr, out oriType);
            }

            var triggerType = (SkillTriggerType)EditorGUILayout.EnumPopup("Type", oriType);
            var triggerStr = Enum.GetName(typeof(SkillTriggerType), triggerType);

            return triggerStr;
        }

        private string[] PropertyStringList(string[] argsList,string[] argsDesc = null)
        {
            //这里应该数据驱动,否则默认使用Args数组,如有特殊说明用自定义
            string[] customArgs = argsDesc;

            if (customArgs != null && customArgs.Length > 0)
            {
                if (argsList == null || argsList.Length != customArgs.Length)
                {
                    argsList = new string[customArgs.Length];
                }
                for (int i = 0;i < customArgs.Length;i++)
                {
                    var desc = customArgs[i];
                    argsList[i] = EditorGUILayout.TextField(desc, argsList[i]);
                }
            }
            else
            {
                SerializedProperty args = serializedObject.FindProperty("args");
                EditorGUILayout.PropertyField(args, new GUIContent("Args"), true);
            }


            return argsList;
        }
    }
}

