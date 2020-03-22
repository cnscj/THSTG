
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetBaseCache : MonoBehaviour
    {
        private Dictionary<string, AssetCacheObject> m_cacheObject = new Dictionary<string, AssetCacheObject>();

        public static AssetBaseCache Create<T>(string name,Transform parent) where T: AssetBaseCache
        {
            GameObject newGO = new GameObject(name);
            newGO.transform.SetParent(parent);
            var comp = newGO.AddComponent<T>();

            return comp;
        }
        public bool Add(string key, Object obj)
        {
            if (m_cacheObject.ContainsKey(key))
            {
                AssetCacheObject cacheObj = new AssetCacheObject(key, obj);

                cacheObj.UpdateTick();
                m_cacheObject.Add(key, cacheObj);
            }
            return false;
        }

        public void Remove(string key)
        {
            if (m_cacheObject.ContainsKey(key))
            {
                //var cacheObj = m_cacheObject[key];
                m_cacheObject.Remove(key);
            }
        }

        public T Get<T>(string key) where T: Object
        {
            if (m_cacheObject.ContainsKey(key))
            {
                var cacheObj = m_cacheObject[key];
                cacheObj.UpdateTick();

                return cacheObj.GetObject<T>();
            }
            return null;
        }

        public void Clear()
        {
            m_cacheObject.Clear();
        }

        private void Update()
        {
            
        }

    }
}