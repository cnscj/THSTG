
using UnityEngine;
using UnityEditor;
using THGame;
using System.Collections.Generic;
using THGame.UI;

namespace THEditor
{
	public class WindowGUI<T> : EditorWindow where T : EditorWindow
    {
        protected SerializedObject m_serializedObject;   //序列化对象
        protected Dictionary<string, List<KeyValuePair<string, SerializedProperty>>> m_propsMaps = new Dictionary<string, List<KeyValuePair<string, SerializedProperty>>>();

        public static void ShowWindow(string title)
        {
            T myWindow = (T)EditorWindow.GetWindow(typeof(T), false, title, false);//创建窗口
            myWindow.Show();//展示

        }

        protected void OnGUI()
        {
            
            m_serializedObject.Update();//属性序列化
            EditorGUI.BeginChangeCheck();//开始检查是否有修改

            EditorGUILayout.BeginVertical();
            OnShow();
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())//结束检查是否有修改
            {
                m_serializedObject.ApplyModifiedProperties();
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
            //使用当前类初始化
            m_serializedObject = new SerializedObject(this);
            Clear();
            OnProps();

        }

        protected SerializedProperty FindProperty(string property)
        {
            return m_serializedObject.FindProperty(property);
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
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        var pair = list[i];
                        if (pair.Key == name)
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
        protected void ShowProperty(SerializedProperty prop, string name = null)
        {
            if (prop != null)
            {
                name = name == null ? prop.displayName : name;
                EditorGUILayout.PropertyField(prop, new GUIContent(name), true);
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

                        ShowProperty(pair.Value, pair.Key);
                    }
                }
            }

        }

        private List<KeyValuePair<string, SerializedProperty>> GetOrCreateList(string group)
        {
            List<KeyValuePair<string, SerializedProperty>> list = null;
            bool ret = m_propsMaps.TryGetValue(group, out list);
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
