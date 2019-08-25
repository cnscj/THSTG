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
        public float cacheDataStayTime = 9f;

        private float m_cacheTimeTemp;

        //缓冲区[key 为绝对路径]
        private Dictionary<string, ResourceCacheDataInfo> cacheDataDic = new Dictionary<string, ResourceCacheDataInfo>();

        //检测缓冲区
        public ResourceCacheDataInfo QueryCache(string path)
        {
            if (cacheDataDic.ContainsKey(path))
            {
                return cacheDataDic[path];
            }
            return null;
        }

        //加入缓冲区
        public void PushCache(string path, Object obj)
        {
            Debug.Log("[ResourceManager]加入缓存:" + path);

            lock (cacheDataDic) {
                if (cacheDataDic.ContainsKey(path))
                {
                    cacheDataDic[path].UpdateTick();
                }
                else
                {
                    ResourceCacheDataInfo info = new ResourceCacheDataInfo(path, obj);
                    cacheDataDic.Add(path, info);
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
                if (iter.Value.startTick + cacheDataStayTime >= Time.realtimeSinceStartup)
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
