
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetCacheManager : MonoSingleton<AssetCacheManager>
    {
        [Header("定时清理缓存间隔(秒):")]
        public float clearCacheCDTime = 10f;

        private Dictionary<string, AssetCacheObjectInfo> m_cacheDataDict = new Dictionary<string, AssetCacheObjectInfo>();//缓冲区[key 为绝对路径]
        private float m_cacheTimeTemp;

        //检测缓冲区
        public AssetCacheObjectInfo Query(string key)
        {
            if (m_cacheDataDict.ContainsKey(key))
            {
                return m_cacheDataDict[key];
            }
            return null;
        }

        //加入缓冲区
        public void Push(string key, Object obj, float stayTime = 30f)
        {
            if (key == null)
                return;

            if (obj == null)
                return;

            lock (m_cacheDataDict)
            {
                if (m_cacheDataDict.ContainsKey(key))
                {
                    m_cacheDataDict[key].UpdateTick();
                }
                else
                {
                    AssetCacheObjectInfo info = new AssetCacheObjectInfo(key, obj);
                    info.stayTime = stayTime;

                    m_cacheDataDict.Add(key, info);
                    info.UpdateTick();
                }
            }
        }

        public void Remove(string key)
        {
            m_cacheDataDict.Remove(key);
        }

        //清空缓冲区
        public void Clear()
        {
            m_cacheDataDict.Clear();
        }

        public void Update()
        {
            if (clearCacheCDTime < 0) return;

            m_cacheTimeTemp += Time.deltaTime;

            if (m_cacheTimeTemp >= clearCacheCDTime)
            {
                PurgeCache();
                m_cacheTimeTemp -= clearCacheCDTime;
            }
        }

        //清理缓冲区
        private void PurgeCache()
        {
            foreach (var iter in m_cacheDataDict.ToList())
            {
                if (iter.Value.CheckRemove())
                {
                    m_cacheDataDict.Remove(iter.Key);
                }
            }
        }
    }
}