
using UnityEngine;
using UnityEditor;
using THGame;
using System.Collections.Generic;
using THGame.UI;

namespace THEditor
{
	public class ScriptGUI<T> : Editor where T : Object
    {
		protected T m_editor;
        protected Dictionary<string, List<KeyValuePair<string, SerializedProperty>>> m_propsMaps = new Dictionary<string, List<KeyValuePair<string, SerializedProperty>>>();

		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginVertical();
			serializedObject.Update();//属性序列化

            OnShow();

            serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndVertical();
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

        protected SerializedProperty FindProperty(string property)
        {
            return serializedObject.FindProperty(property);
        }

        protected SerializedProperty GetProperty(string group, string name)
        {
            if (group != null)
            {
                List<KeyValuePair<string, SerializedProperty>> list = null;
                bool ret = m_propsMaps.TryGetValue(group, out list);
                if (ret)
                {
                    foreach (var pair in list)
                    {

                        if (pair.Key == name)
                        {
                            return pair.Value;
                        }
                    }
                }
            }
            return null;
        }

        protected SerializedProperty AddProperty(string property, string group, string name = null)
		{
            SerializedProperty prop = FindProperty(property);
            if (prop != null)
            {
                name = name != null ? name : property;
                KeyValuePair<string, SerializedProperty> pair = new KeyValuePair<string, SerializedProperty>(name, prop);
                var list = GetOrCreateList(group);
                list.Add(pair);
            }
            return prop;
        }

        protected void RemoveProperty(string group, string name = null)
        {
            if (name != null)
            {
                List<KeyValuePair<string, SerializedProperty>> list = null;
                bool ret = m_propsMaps.TryGetValue(group, out list);
                if (ret)
                {
                    for(int i = list.Count - 1; i >= 0; i--)
                    {
                        var pair = list[i];
                        if(pair.Key == name)
                        {
                            list.Remove(pair);
                        }
                    }
                }
            }
            else
            {
                m_propsMaps.Remove(group);
            }
            
        }
        protected void ShowPropertys(string name, SerializedProperty prop)
        {
            EditorGUILayout.PropertyField(prop, new GUIContent(name), true);
        }
        protected void ShowPropertys(string group = null)
		{
   
            if (group != null)
            {
                List<KeyValuePair<string, SerializedProperty>> list = null;
                bool ret = m_propsMaps.TryGetValue(group, out list);
                if (ret)
                {
                    foreach (var pair in list)
                    {

                        ShowPropertys(pair.Key, pair.Value);
                    }
                }
            }
            
        }

        private List<KeyValuePair<string, SerializedProperty>> GetOrCreateList(string group)
        {
            List<KeyValuePair<string, SerializedProperty>> list = null;
            bool ret = m_propsMaps.TryGetValue(group,out list);
            if (!ret)
            {
                list = new List<KeyValuePair<string, SerializedProperty>>();
                m_propsMaps.Add(group, list);
            }
            return list;
        }

        public virtual void Clear()
		{
            m_propsMaps.Clear();
        }

	}
}
