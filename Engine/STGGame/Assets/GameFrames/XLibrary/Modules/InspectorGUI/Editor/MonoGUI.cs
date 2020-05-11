
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace XLibEditor
{
    public class MonoGUI<T> : Editor where T : Object
    {
        public static readonly string defauleGroup = "_default";
        public class PropertyInfo
        {
            public string propertyName;
            public SerializedProperty serializedProperty;
            public string labelName;
            public string groupName;
        }

		protected T m_editor;
        protected Dictionary<string, List<PropertyInfo>> m_propsMaps = new Dictionary<string, List<PropertyInfo>>();

		public override void OnInspectorGUI()
		{
			
			serializedObject.Update();//属性序列化
            EditorGUI.BeginChangeCheck();//开始检查是否有修改

            EditorGUILayout.BeginVertical();
            OnShow();
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())//结束检查是否有修改
            {
                serializedObject.ApplyModifiedProperties();
            }
			
		}

        protected virtual void OnProps()
        {

        }

        protected virtual void OnShow()
        {
            
        }

		void OnEnable()
		{
			m_editor = (T)target;
			Clear();
            OnProps();

        }

        protected SerializedProperty FindProperty(string propertyName)
        {
            return serializedObject.FindProperty(propertyName);
        }

        protected SerializedProperty GetProperty(string propertyName, string groupName = null)
        {
            groupName = string.IsNullOrEmpty(groupName) ? defauleGroup : groupName;
            if (groupName != null)
            {
                List<PropertyInfo> list = null;
                bool ret = m_propsMaps.TryGetValue(groupName, out list);
                if (ret)
                {
                    foreach (var info in list)
                    {

                        if (info.propertyName == propertyName)
                        {
                            return info.serializedProperty;
                        }
                    }
                }
            }
            return null;
        }

        protected SerializedProperty AddProperty(string propertyName, string labelName = null, string groupName = null)
		{
            SerializedProperty prop = FindProperty(propertyName);
            if (prop != null)
            {
                groupName = string.IsNullOrEmpty(groupName) ? defauleGroup : groupName;
                labelName = string.IsNullOrEmpty(labelName) ? propertyName : labelName;
                PropertyInfo info = new PropertyInfo();
                info.propertyName = propertyName;
                info.groupName = groupName;
                info.serializedProperty = prop;
                info.labelName = labelName;

                var list = GetOrCreateList(groupName);
                list.Add(info);
            }
            return prop;
        }

        protected void RemoveProperty(string propertyName, string groupName = null)
        {
            groupName = string.IsNullOrEmpty(groupName) ? defauleGroup : groupName;
            if (propertyName != null)
            {
                List<PropertyInfo> list = null;
                bool ret = m_propsMaps.TryGetValue(groupName, out list);
                if (ret)
                {
                    for(int i = list.Count - 1; i >= 0; i--)
                    {
                        var info = list[i];
                        if(info.propertyName == propertyName)
                        {
                            list.Remove(info);
                        }
                    }
                }
            }
            else
            {
                m_propsMaps.Remove(groupName);
            }
            
        }
        protected void ShowProperty(SerializedProperty serializedProperty, string labelName = null)
        {
            if (serializedProperty != null)
            {
                labelName = labelName == null ? serializedProperty.displayName : labelName;
                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(labelName), true);
            }
            
        }
        protected void ShowProperty(string propertyName, string groupName = null)
        {
            groupName = string.IsNullOrEmpty(groupName) ? defauleGroup : groupName;
            if (groupName != null)
            {
                List<PropertyInfo> list = null;
                bool ret = m_propsMaps.TryGetValue(groupName, out list);
                if (ret)
                {
                    foreach (var info in list)
                    {

                        if (info.propertyName == propertyName)
                        {
                            ShowProperty(info.serializedProperty, info.labelName);
                            return;
                        }
                    }
                }
            }
        }

        protected void ShowPropertys(string groupName = null)
		{
            groupName = string.IsNullOrEmpty(groupName) ? defauleGroup : groupName;
            if (groupName != null)
            {
                List<PropertyInfo> list = null;
                bool ret = m_propsMaps.TryGetValue(groupName, out list);
                if (ret)
                {
                    foreach (var info in list)
                    {

                        ShowProperty(info.serializedProperty, info.labelName);
                    }
                }
            }
            
        }

        private List<PropertyInfo> GetOrCreateList(string groupName)
        {
            List<PropertyInfo> list = null;
            bool ret = m_propsMaps.TryGetValue(groupName, out list);
            if (!ret)
            {
                list = new List<PropertyInfo>();
                m_propsMaps.Add(groupName, list);
            }
            return list;
        }

        public virtual void Clear()
		{
            m_propsMaps.Clear();
        }

	}
}
