
using UnityEngine;
using Object = UnityEngine.Object;
using XLibGame;

namespace ASGame
{
    //缓冲数据信息
    public class AssetObjectCacheObject : BaseRef
    {
        public readonly string cacheName;       //所属缓冲数据名称

        public readonly string cacheKey;         //缓冲数据名称
        public readonly Object cacheObj;        //缓冲物体

        public float stayTime = -1f;            //最长驻留时间s
        private float m_updateTick;             //最后一次使用时间

        public AssetObjectCacheObject(string cache, string key, Object obj)
        {
            cacheName = cache;
            cacheKey = key;
            cacheObj = obj;
        }

        //刷新进入缓冲区时间
        public void UpdateTick()
        {
            m_updateTick = Time.realtimeSinceStartup;
        }

        public bool CheckRemove()
        {
            if (stayTime > 0)
            {
                if (RefCount <= 0)
                {
                    if (m_updateTick + stayTime <= Time.realtimeSinceStartup)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (RefCount <= 0)
                {
                    return true;
                }
            }
            return false;
        }

        public T GetObject<T>() where T : Object
        {
            return cacheObj as T;
        }

    }

}
