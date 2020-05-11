
using System.Collections.Generic;
using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetObjectCache : AssetBaseCache
    {
        private Dictionary<string, AssetObjectCacheObject> m_cacheObject = new Dictionary<string, AssetObjectCacheObject>();
        private Queue<string> m_releaseQueue = new Queue<string>();

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
                AssetObjectCacheObject cacheObj = new AssetObjectCacheObject(cacheName, key, obj);

                cacheObj.UpdateTick();
                m_cacheObject.Add(key, cacheObj);
            }
            else
            {
                if (isReplace)
                {
                    AssetObjectCacheObject cacheObj = new AssetObjectCacheObject(cacheName, key, obj);
                    m_cacheObject[key] = cacheObj;
                    return;
                }
            }
            return;
        }

        public override void Remove(string key)
        {
            if (m_cacheObject.ContainsKey(key))
            {
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
            return null;
        }

        public override void Clear()
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
            foreach (var cachePair in m_cacheObject)
            {
                if (cachePair.Value.CheckRemove())
                {
                    m_releaseQueue.Enqueue(cachePair.Key);
                }
            }
        }

        private void UpdateRelease()
        {
            while (m_releaseQueue.Count > 0)
            {
                var objKey = m_releaseQueue.Dequeue();
                m_cacheObject.Remove(objKey);
            }
        }
    }
}