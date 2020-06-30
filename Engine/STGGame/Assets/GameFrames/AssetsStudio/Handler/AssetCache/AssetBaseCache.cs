
using UnityEngine;
using Object = System.Object;
namespace ASGame
{
    public abstract class AssetBaseCache : MonoBehaviour
    {
        protected string cacheName;
       
        public static AssetBaseCache Create<T>(string name, Transform parent = null) where T: AssetBaseCache
        {
            GameObject newGO = new GameObject(name);
            var comp = newGO.AddComponent<T>();
            comp.cacheName = name;

            if (parent != null)
            {
                newGO.transform.SetParent(parent, false);
            }
            return comp;
        }

        public string GetName()
        {
            return cacheName;
        }

        ///
        public abstract void Add(string key, Object obj, bool isReplace = false);

        public abstract void Remove(string key);

        public abstract T Get<T>(string key);

        public virtual void Clear() { }
    }
}