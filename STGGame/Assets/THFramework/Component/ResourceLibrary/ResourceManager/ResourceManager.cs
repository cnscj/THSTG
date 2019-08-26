using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Object = UnityEngine.Object;
using THGame.Package;

namespace THGame
{

    //资源管理器 (资源管理器 仅会把资源加载入内存)
    public class ResourceManager : MonoSingleton<ResourceManager>
    {
        [Header("定时清理缓存间隔(秒):")]
        public float clearCacheDuration = 10f;
        [Header("缓存数据驻留时间(秒)")]
        public float cacheDataStayTime = 30f;

        private float m_cacheTimeTemp;

        //缓冲区[key 为绝对路径]
        private Dictionary<string, ResourceCacheDataInfo> cacheDataDic = new Dictionary<string, ResourceCacheDataInfo>();

        //检测缓冲区
        public ResourceCacheDataInfo QueryCache(string key)
        {
            if (cacheDataDic.ContainsKey(key))
            {
                return cacheDataDic[key];
            }
            return null;
        }

        //加入缓冲区
        public void PushCache(string key, Object obj)
        {
            Debug.Log("[ResourceManager]加入缓存:" + key);

            lock (cacheDataDic)
            {
                if (cacheDataDic.ContainsKey(key))
                {
                    cacheDataDic[key].UpdateTick();
                }
                else
                {
                    ResourceCacheDataInfo info = new ResourceCacheDataInfo(key, obj);
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
        private void updateCache()
        {
            Debug.Log("[ResourceManager]清理缓存");
            foreach (var iter in cacheDataDic.ToList())
            {
                if (iter.Value.startTick + cacheDataStayTime <= Time.realtimeSinceStartup)
                {
                    Debug.Log("过期清理:" + iter.Value.cacheName);
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
                updateCache();
                m_cacheTimeTemp -= clearCacheDuration;
            }
        }

    }

}
