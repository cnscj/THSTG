
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
namespace ASGame
{
    public class AssetObjectCache : AssetBaseCache
    {
        public float checkFrequency = 5f;      //检查频度

        private Dictionary<string, AssetObjectCacheObject> m_cacheObject = new Dictionary<string, AssetObjectCacheObject>();
        private Queue<string> m_releaseQueue = new Queue<string>();
        private float m_lastCheckTime;

        public bool Has(string key)
        {
            return m_cacheObject.ContainsKey(key);
        }

        public override void Add(string key, Object obj, bool isReplace = false)
        {
            if (obj == null)
                return;

            if (!m_cacheObject.ContainsKey(key))
            {
                AssetObjectCacheObject cacheObj = AssetObjectCacheObject.Create(cacheName, key, obj, transform);

                cacheObj.UpdateTick();
                m_cacheObject.Add(key, cacheObj);
            }
            else
            {
                if (isReplace)
                {
                    Remove(key);

                    AssetObjectCacheObject cacheObj = AssetObjectCacheObject.Create(cacheName, key, obj, transform);
                    m_cacheObject[key] = cacheObj;
                    return;
                }
            }
            return;
        }

        public override void Remove(string key)
        {
            if (m_cacheObject.TryGetValue(key,out var cacheObj))
            {
                Destroy(cacheObj.gameObject);
                m_cacheObject.Remove(key);
            }
        }

        public override T Get<T>(string key)
        {
            if (m_cacheObject.ContainsKey(key))
            {
                var cacheObj = m_cacheObject[key];
                cacheObj.UpdateTick();

                return cacheObj.GetObject<T>();
            }
            return default;
        }

        public override void Clear()
        {
            foreach(var cacheObj in m_cacheObject.Values)
            {
                Destroy(cacheObj.gameObject);
            }
            m_cacheObject.Clear();
        }

        public AssetObjectCacheObject GetCacheObject(string key)
        {
            if (m_cacheObject.TryGetValue(key, out var cacheObj))
            {
                return cacheObj;
            }
            return default;
        }

        //
        private void Update()
        {
            UpdateCheck();
            UpdateRelease();
        }

        private void UpdateCheck()
        {
            if (Time.realtimeSinceStartup - m_lastCheckTime < checkFrequency)
                return;

            foreach (var cachePair in m_cacheObject)
            {
                var cacheObj = cachePair.Value;
                if (cacheObj.CheckRemove())
                {
                    m_releaseQueue.Enqueue(cachePair.Key);
                }
            }
            m_lastCheckTime = Time.realtimeSinceStartup;
        }

        private void UpdateRelease()
        {
            while (m_releaseQueue.Count > 0)
            {
                var objKey = m_releaseQueue.Dequeue();
                Remove(objKey);
            }
        }
    }
}