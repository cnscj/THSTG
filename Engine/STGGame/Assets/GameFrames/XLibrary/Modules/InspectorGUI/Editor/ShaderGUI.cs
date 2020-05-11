
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace XLibEditor
{
	public class ShaderGUI<T> : UnityEditor.ShaderGUI
    {
        public static readonly string defauleGroup = "_default";

        protected MaterialEditor m_editor;
        protected MaterialProperty[] m_props;
        protected Material m_material;
        protected Dictionary<string, List<KeyValuePair<string, MaterialProperty>>> m_propsMaps = new Dictionary<string, List<KeyValuePair<string, MaterialProperty>>>();
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            m_props = props;
            m_editor = materialEditor;
            m_material = materialEditor.target as Material;
        }


        protected virtual void OnProps()
        {

        }

        protected virtual void OnShow()
        {

        }

        protected MaterialProperty FindProperty(string property)
        {
            return ShaderGUI.FindProperty(property, m_props);
        }

        protected MaterialProperty GetProperty(string group, string name)
        {
            if (group != null)
            {
                List<KeyValuePair<string, MaterialProperty>> list = null;
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

        protected MaterialProperty AddProperty(string property, string group, string name = null)
        {
            MaterialProperty prop = FindProperty(property);
            if (prop != null)
            {
                name = name != null ? name : property;
                KeyValuePair<string, MaterialProperty> pair = new KeyValuePair<string, MaterialProperty>(name, prop);
                var list = GetOrCreateList(group);
                list.Add(pair);
            }
            return prop;
        }

        protected void RemoveProperty(string group, string name = null)
        {
            if (name != null)
            {
                List<KeyValuePair<string, MaterialProperty>> list = null;
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

        protected void ShowProperty(string name, MaterialProperty prop)
        {
            m_editor.ShaderProperty(prop, name);
        }

        protected void ShowPropertys(string group)
        {
            if (group != null)
            {
                List<KeyValuePair<string, MaterialProperty>> list = null;
                bool ret = m_propsMaps.TryGetValue(group, out list);
                if (ret)
                {
                    foreach (var pair in list)
                    {
                        ShowProperty(pair.Key, pair.Value);
                    }
                }
            }
        }


        private List<KeyValuePair<string, MaterialProperty>> GetOrCreateList(string group)
        {
            List<KeyValuePair<string, MaterialProperty>> list = null;
            bool ret = m_propsMaps.TryGetValue(group, out list);
            if (!ret)
            {
                list = new List<KeyValuePair<string, MaterialProperty>>();
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
