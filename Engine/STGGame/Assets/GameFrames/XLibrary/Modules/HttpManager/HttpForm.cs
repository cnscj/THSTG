
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace XLibGame
{
    public class HttpForm
    {
        private List<Tuple<string, object>> m_params;

        public HttpForm()
        {
        }

        public object this[string key]
        {
            set
            {
                m_params = m_params ?? new List<Tuple<string, object>>();
                m_params.Add(new Tuple<string, object>(key, value));
            }
        }


        public void Add(string field, object data)
        {
            if (m_params == null)
                return;

            m_params.Add(new Tuple<string, object>(field, data));
        }

        public void Remove(string field)
        {
            if (m_params == null)
                return;

            for(int i = 0;i < m_params.Count; i++)
            {
                var pair = m_params[i];
                if (string.IsNullOrEmpty(field))
                {
                    if (pair.Item1 == field)
                    {
                        m_params.Remove(pair);
                    }
                }
                else
                {
                    if (pair.Item2.ToString() == field)
                    {
                        m_params.Remove(pair);
                    }
                }
                
            }
        }
        public void RemoveLast()
        {
            if (m_params == null)
                return;

            m_params.RemoveAt(m_params.Count - 1);
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

        public string ToGetData(string url = null)
        {
            string ret = string.IsNullOrEmpty(url) ? "" : url;
            StringBuilder data = new StringBuilder();

            string args = ToString();
            data.Append(args);

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

        public WWWForm ToPostData(string url = null)
        {
            WWWForm ret = new WWWForm();
            if (m_params != null && m_params.Count > 0)
            {
                foreach (var pair in m_params)
                {
                    if (!string.IsNullOrEmpty(pair.Item1))
                    {
                        if (pair.Item2 is byte[])
                            ret.AddBinaryData(pair.Item1, pair.Item2 as byte[]);
                        else
                            ret.AddField(pair.Item1, pair.Item2.ToString());
                    }
                   
                }
            }
            return ret;
        }

        public override string ToString()
        {
            StringBuilder data = new StringBuilder();

            if (m_params != null && m_params.Count > 0)
            {
                foreach (var pair in m_params)
                {
                    if (string.IsNullOrEmpty(pair.Item1))
                    {
                        data.Remove(data.Length - 1, 1);
                        data.Append(pair.Item2.ToString() + "&");
                    }
                    else
                    {
                        data.Append(pair.Item1 + "=");
                        data.Append(pair.Item2.ToString() + "&");
                    }
                }
                data.Remove(data.Length - 1, 1);
            }

            return data.ToString();
        }

    }
}