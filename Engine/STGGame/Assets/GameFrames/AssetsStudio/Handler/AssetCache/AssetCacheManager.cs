
using System.Collections.Generic;
using XLibrary.Package;
using Object = System.Object;
namespace ASGame
{
    public class AssetCacheManager : MonoSingleton<AssetCacheManager>
    {
        private static readonly string DEFAULT_CACHE = "ObjectCache";
        private Dictionary<string, AssetBaseCache> m_cachesMap = new Dictionary<string, AssetBaseCache>();

        public AssetBaseCache GetOrCreateCache<T>(string cacheName) where T : AssetBaseCache
        {
            if (!m_cachesMap.ContainsKey(cacheName))
            {
                AssetBaseCache cache = AssetBaseCache.Create<T>(cacheName, transform);
                m_cachesMap.Add(cacheName, cache);
            }
            return m_cachesMap[cacheName];
        }

        public AssetBaseCache AddCache(string cacheName)
        {
            return GetOrCreateCache<AssetObjectCache>(cacheName);
        }

        public AssetBaseCache AddCache<T>(string cacheName) where T : AssetBaseCache
        {
            return GetOrCreateCache<T>(cacheName);
        }

        public AssetBaseCache GetCache(string cacheName)
        {
            if (m_cachesMap.ContainsKey(cacheName))
            {
                return m_cachesMap[cacheName];
            }
            return null;
        }

        public void ClearCache(string cacheName)
        {
            if (m_cachesMap.TryGetValue(cacheName, out var cache))
            {
                cache.Clear();
            }
        }

        public void RemoveCache(string cacheName)
        {
            if (m_cachesMap.TryGetValue(cacheName, out var cache))
            {
                Destroy(cache.gameObject);
                m_cachesMap.Remove(cacheName);
            }
        }

        public void DisposeAll()
        {
            foreach(var cache in m_cachesMap.Values)
            {
                cache.Clear();
            }
            m_cachesMap.Clear();
        }

        ///////
        public T GetObject<T>(string objKey)
        {
            if (m_cachesMap.TryGetValue(DEFAULT_CACHE, out var cache))
            {
                return cache.Get<T>(objKey);
            }
            return default;
        }

        public void AddObject(string objKey, Object obj, bool isReplace = false)
        {
            var cache = GetOrCreateCache<AssetObjectCache>(DEFAULT_CACHE);
            cache.Add(objKey, obj, isReplace);
        }

        public void RemoveObject(string objKey)
        {
            if (m_cachesMap.TryGetValue(DEFAULT_CACHE, out var cache))
            {
                cache.Remove(objKey);
            }
        }

        public void ClearObjects()
        {
            ClearCache(DEFAULT_CACHE);
        }

    }
}