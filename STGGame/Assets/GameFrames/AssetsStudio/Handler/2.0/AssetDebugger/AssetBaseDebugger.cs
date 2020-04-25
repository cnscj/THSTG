using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public abstract class AssetBaseDebugger : MonoBehaviour
    {
        public static AssetBaseDebugger CreateDebugger<T>(Transform parent, string name = "_DEBUGGER_") where T : AssetBaseDebugger
        {
            GameObject debugger = new GameObject(name);
            debugger.transform.SetParent(parent);
            return debugger.AddComponent<T>();
        }

        public abstract void To(string key);
        public abstract void Add(string key);
        public abstract void Remove(string key);
        public abstract void Clear();
    }
}
