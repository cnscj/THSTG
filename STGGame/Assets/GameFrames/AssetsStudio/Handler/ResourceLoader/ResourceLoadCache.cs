using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;
using XLibrary.Package;

namespace ASGame
{

    //资源管理器 (资源管理器 仅会把资源加载入内存)
    public class ResourceLoadCache : MonoSingleton<ResourceLoadCache>
    {
        [Header("定时清理缓存间隔(秒):")]
        public float clearCacheDuration = 10f;

        private float m_cacheTimeTemp;

        //缓冲区[key 为绝对路径]
        private Dictionary<string, ResourceLoadCacheDataInfo> cacheDataDic = new Dictionary<string, ResourceLoadCacheDataInfo>();

        //检测缓冲区
        public ResourceLoadCacheDataInfo QueryCache(string key)
        {
            if (cacheDataDic.ContainsKey(key))
            {
                return cacheDataDic[key];
            }
            return null;
        }

        //加入缓冲区
        public void PushCache(string key, Object obj, float stayTime = 30f)
        {
            if (key == null)
                return;

            if (obj == null)
                return;

            lock (cacheDataDic)
            {
                if (cacheDataDic.ContainsKey(key))
                {
                    cacheDataDic[key].UpdateTick();
                }
                else
                {
                    ResourceLoadCacheDataInfo info = new ResourceLoadCacheDataInfo(key, obj, stayTime);
                    cacheDataDic.Add(key, info);
                    info.UpdateTick();
                }
            }
        }

        //清空缓冲区
        public void RemoveCache()
        {
            cacheDataDic.Clear();
        }

        //清理缓冲区
        private void UpdateCache()
        {
            foreach (var iter in cacheDataDic.ToList())
            {
                if (iter.Value.startTick + iter.Value.stayTime <= Time.realtimeSinceStartup)
                {
                    cacheDataDic.Remove(iter.Key);
                }
            }
        }


        private void Update()
        {
            if (clearCacheDuration < 0) return;

            m_cacheTimeTemp += Time.deltaTime;

            if (m_cacheTimeTemp >= clearCacheDuration)
            {
                UpdateCache();
                m_cacheTimeTemp -= clearCacheDuration;
            }
        }

    }

}
