using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public class RequestForm
    {
        private Dictionary<string, object> m_params;

        public RequestForm()
        {
            m_params = new Dictionary<string, object>();
        }

        public RequestForm(Dictionary<string, object> form)
        {
            m_params = new Dictionary<string, object>();
            if (form != null)
            {
                foreach (var pair in form)
                {
                    Add(pair.Key, pair.Value);
                }
            }

        }

        public RequestForm(RequestForm form):this(form.m_params)
        {

        }

        public void Add(string field, object data)
        {
            if (m_params == null)
                return;

            m_params[field] = data;
        }

        public void Remove(string field)
        {
            if (m_params == null)
                return;

            m_params.Remove(field);
        }

        public void Clear()
        {
            if (m_params == null)
                return;

            m_params.Clear();
        }

        public int Count()
        {
            if (m_params == null)
                return 0;
            return m_params.Count;
        }

        public override string ToString()
        {
            WWWForm ret = new WWWForm();
            if (m_params != null && m_params.Count > 0)
            {
                foreach (var item in m_params)
                {
                    if (item.Value is byte[])
                        ret.AddBinaryData(item.Key, item.Value as byte[]);
                    else
                        ret.AddField(item.Key, item.Value.ToString());
                }
            }
            return ret.ToString();
        }

        public Dictionary<string, object> GetParams()
        {
            return m_params;
            
        }
    }
}

