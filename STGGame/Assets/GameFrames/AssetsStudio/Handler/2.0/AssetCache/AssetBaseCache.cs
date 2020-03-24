
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetBaseCache : MonoBehaviour
    {
        private string cacheName;
        private Dictionary<string, AssetCacheObject> m_cacheObject = new Dictionary<string, AssetCacheObject>();
        private Queue<string> m_releaseQueue = new Queue<string>();

        public static AssetBaseCache Create<T>(string name, Transform parent) where T: AssetBaseCache
        {
            GameObject newGO = new GameObject(name);
            newGO.transform.SetParent(parent);
            var comp = newGO.AddComponent<T>();
            comp.cacheName = name;
            return comp;
        }

        public bool IsContains(string key)
        {
            return m_cacheObject.ContainsKey(key);
        }

        public bool Add(string key, Object obj, bool isReplace = false)
        {
            if (!m_cacheObject.ContainsKey(key))
            {
                AssetCacheObject cacheObj = new AssetCacheObject(cacheName, key, obj);

                cacheObj.UpdateTick();
                m_cacheObject.Add(key, cacheObj);
            }else
            {
                if (isReplace)
                {
                    AssetCacheObject cacheObj = new AssetCacheObject(cacheName, key, obj);
                    m_cacheObject[key] = cacheObj;
                    return true;
                }
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
            UpdateCheck();
            UpdateRelease();
        }

        private void UpdateCheck()
        {
            foreach(var cachePair in m_cacheObject)
            {
                if (cachePair.Value.CheckRemove())
                {
                    m_releaseQueue.Enqueue(cachePair.Key);
                }
            }
        }

        private void UpdateRelease()
        {
            while(m_releaseQueue.Count > 0)
            {
                var objKey = m_releaseQueue.Dequeue();
                //if (m_cacheObject.TryGetValue(objKey, out var cacheObj))
                //{
                    m_cacheObject.Remove(objKey);
                //}
            }
        }
    }
}