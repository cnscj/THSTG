using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;
using XLibGame;

namespace ASGame
{
    //缓冲数据信息
    public class AssetCacheObject
    {
        public readonly string objName;         //缓冲数据名称
        public readonly Object cacheObj;        //缓冲物体
        public readonly string cacheName;       //缓冲数据名称

        public float stayTime = -1f;            //最长驻留时间s

        private float m_updateTick;             //最后一次使用时间

        public AssetCacheObject(string cache, string name, Object obj)
        {
            cacheName = cache;
            objName = name;
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
                if (m_updateTick + stayTime <= Time.realtimeSinceStartup)
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
