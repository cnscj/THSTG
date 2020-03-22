
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetCacheManager : MonoSingleton<AssetCacheManager>
    {
        private Dictionary<string, AssetBaseCache> m_cachesMap = new Dictionary<string, AssetBaseCache>();

        public AssetBaseCache AddCache(string cacheName)
        {
            return GetOrCreateCache(cacheName);
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

        public void AddObject2Cache(string cacheName, string objKey, Object obj)
        {
            var cache = GetOrCreateCache(cacheName);
            cache.Add(objKey, obj);
        }

        private AssetBaseCache GetOrCreateCache(string cacheName)
        {
            if (!m_cachesMap.ContainsKey(cacheName))
            {
                AssetBaseCache cache = AssetBaseCache.Create<AssetBaseCache>(cacheName, transform);
                m_cachesMap.Add(cacheName, cache);
            }
            return m_cachesMap[cacheName];
        }
    }
}