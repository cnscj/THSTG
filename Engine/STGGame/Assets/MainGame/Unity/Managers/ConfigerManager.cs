
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary.Package;

namespace STGU3D
{
    public class ConfigerManager : MonoSingleton<ConfigerManager>
    {
        private Dictionary<string, string> m_cache;
        public ConfigerManager()
        {
            m_cache = new Dictionary<string, string>();
        }

        public string LoadConfig(string code)
        {
            string content;
            if (m_cache.ContainsKey(code))
            {
                content = m_cache[code];
            }
            else
            {
                content = AssetManager2.GetInstance().LoadConfigSync(code);
                if (!string.IsNullOrEmpty(content))
                {
                    m_cache.Add(code, content);
                }
            }
            

            return content;
        }
    }
}
