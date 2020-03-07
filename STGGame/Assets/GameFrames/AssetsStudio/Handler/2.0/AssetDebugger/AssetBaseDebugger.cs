using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public abstract class AssetBaseDebugger : MonoBehaviour
    {
        public abstract void Push(string key, string msg);
        public abstract void Remove(string key);

        public static AssetBaseDebugger CreateDebugger<T>(string name, Transform parent) where T : AssetBaseDebugger
        {
            GameObject debugger = new GameObject(name);
            debugger.transform.SetParent(parent);
            return debugger.AddComponent<T>();
        }
    }
}
