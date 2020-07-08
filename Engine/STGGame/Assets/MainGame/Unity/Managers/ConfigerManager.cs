
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary;
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

        public CSVTable LoadConfig(string code)
        {
            CSVTable table = new CSVTable();
            if (m_cache.ContainsKey(code))
            {
                string content = m_cache[code];
                table.Parse(content);
            }
            else
            {
                var content = AssetManager.GetInstance().LoadConfig(code);
                if (!string.IsNullOrEmpty(content))
                {
                    m_cache.Add(code, content);
                    table.Parse(content);
                }
            }

            return table;
        }
    }
}
