using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASGame
{
    public class AssetDebuggerManager : MonoSingleton<AssetDebuggerManager>
    {
        private static readonly string KEY_LOADER = "loader";
        private Dictionary<string, AssetBaseDebugger> m_debuggerMap;
        public AssetBaseDebugger Loader()
        {
            return GetOrCreateDebugger(KEY_LOADER);
        }


        private AssetBaseDebugger GetOrCreateDebugger(string key)
        {
            m_debuggerMap = m_debuggerMap ?? new Dictionary<string, AssetBaseDebugger>();
            if (!m_debuggerMap.ContainsKey(key))
            {
#if UNITY_EDITOR
                m_debuggerMap[key] = AssetBaseDebugger.CreateDebugger<AssetInstanceDebugger>(transform);
#else
                m_debuggerMap[key] = AssetBaseDebugger.CreateDebugger<AssetNullDebugger>(transform);
#endif
            }
            return m_debuggerMap[key];
        }
    }
}
