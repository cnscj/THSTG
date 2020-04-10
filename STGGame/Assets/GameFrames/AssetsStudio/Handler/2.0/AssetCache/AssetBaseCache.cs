
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
namespace ASGame
{
    public abstract class AssetBaseCache : MonoBehaviour
    {
        protected string cacheName;
       
        public static AssetBaseCache Create<T>(string name, Transform parent) where T: AssetBaseCache
        {
            GameObject newGO = new GameObject(name);
            newGO.transform.SetParent(parent);
            var comp = newGO.AddComponent<T>();
            comp.cacheName = name;
            return comp;
        }

        public string GetName()
        {
            return cacheName;
        }

        public abstract bool Add(string key, Object obj, bool isReplace = false);

        public abstract void Remove(string key);

        public abstract T Get<T>(string key) where T : Object;

        public virtual void Clear() { }
    }
}