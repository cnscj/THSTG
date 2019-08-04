
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

            OnIGUI();

            serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndVertical();
		}

        protected virtual void OnProps()
        {

        }

        protected virtual void OnIGUI()
        {
            
        }

		void OnEnable()
		{
			m_editor = (T)target;
			Clear();
            OnProps();

        }


		protected void AddPropertys(string group, string name, string property)
		{
            SerializedProperty prop = serializedObject.FindProperty(property);
            if (prop != null)
            {
                KeyValuePair<string, SerializedProperty> pair = new KeyValuePair<string, SerializedProperty>(name, prop);
                var list = GetOrCreateList(group);
                list.Add(pair);
            }
        }

        protected void RemovePropertys(string group, string name = null)
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
                        
                        EditorGUILayout.PropertyField(pair.Value, new GUIContent(pair.Key), true);
                    }
                }
            }
            else
            {
                
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

        void Clear()
		{
            m_propsMaps.Clear();
        }

	}
}
