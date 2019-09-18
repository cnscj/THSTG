
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace XLibraryEditor
{
	public class WindowGUI<T> : EditorWindow where T : EditorWindow
    {
        protected List<SerializedObject> m_serializedObjectList = new List<SerializedObject>();   //序列化对象列表
        protected Dictionary<string, List<KeyValuePair<string, SerializedProperty>>> m_propsMaps = new Dictionary<string, List<KeyValuePair<string, SerializedProperty>>>();

        public static void ShowWindow(string title)
        {
            T myWindow = (T)EditorWindow.GetWindow(typeof(T), false, title, false);//创建窗口
            myWindow.Show();//展示

        }

        protected void OnGUI()
        {
            
            foreach (var serializedObject in m_serializedObjectList)
            {
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.BeginChangeCheck();//开始检查是否有修改

            EditorGUILayout.BeginVertical();
            OnShow();
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())//结束检查是否有修改
            {
                foreach (var serializedObject in m_serializedObjectList)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
            
        }

        protected virtual void OnObjs()
        {

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
            Clear();
            AddObject(this);
            OnObjs();
            OnProps();

        }
        protected SerializedObject AddObject(Object obj)
        {
            SerializedObject serializedObject = new SerializedObject(obj);
            if (serializedObject != null)
            {
                m_serializedObjectList.Add(serializedObject);
                return serializedObject;
            }
            return null;
        }

        protected SerializedProperty FindProperty(string property)
        {
            foreach(var serializedObject in m_serializedObjectList)
            {
                SerializedProperty prop = serializedObject.FindProperty(property);
                if (prop != null)
                {
                    return prop;
                }
            }
            return null;
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
            m_serializedObjectList.Clear();
            m_propsMaps.Clear();
        }

        private void OnDestroy()
        {
            foreach (var serializedObject in m_serializedObjectList)
            {
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
           
            AssetDatabase.SaveAssets();
        }
    }
}
