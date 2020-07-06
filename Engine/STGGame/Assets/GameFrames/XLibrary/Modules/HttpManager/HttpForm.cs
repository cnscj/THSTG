
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace XLibGame
{
    public class HttpForm
    {
        private Dictionary<string, object> m_params;

        public HttpForm()
        {
            m_params = new Dictionary<string, object>();
        }

        public HttpForm(Dictionary<string, object> form)
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

        public HttpForm(HttpForm form) : this(form.m_params)
        {

        }

        public object this[string key]
        {
            get => m_params != null ? m_params[key] : null;
            set
            {

                m_params = m_params ?? new Dictionary<string, object>();
                m_params[key] = value;
            }
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

        public string ToGetData(string url)
        {
            string ret = url;
            StringBuilder data = new StringBuilder();

            if (m_params != null && m_params.Count > 0)
            {
                foreach (var item in m_params)
                {
                    data.Append(item.Key + "=");
                    data.Append(item.Value.ToString() + "&");
                }
                data.Remove(data.Length - 1, 1);
            }

            if (url.IndexOf("?") == -1)
            {
                url += "?";
            }
            else
            {
                url += "&";
            }

            ret = url + data.ToString();

            return ret;
        }

        public WWWForm ToPostData(string url)
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
            return ret;
        }


        public Dictionary<string, object> GetParams()
        {
            return m_params;

        }
    }
}