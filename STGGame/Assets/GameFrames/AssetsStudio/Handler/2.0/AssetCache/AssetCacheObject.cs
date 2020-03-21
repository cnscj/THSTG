using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;
using XLibrary.Package;

namespace ASGame
{
    //缓冲数据信息
    public class AssetCacheObject
    {
        public float stayTime = 30f;      //0引用最长驻留时间

        private Object m_cacheObj;     //缓冲物体
        private string m_cacheName;    //缓冲数据名称
        private float m_startTick;     //进入缓冲区时间
        private int m_refCount;        //引用次数

        public AssetCacheObject(string name, Object obj)
        {
            m_cacheName = name;
            m_cacheObj = obj;
        }

        //刷新进入缓冲区时间
        public void UpdateTick()
        {
            m_startTick = Time.realtimeSinceStartup;
        }

        public bool CheckRemove()
        {
            if (m_startTick + stayTime <= Time.realtimeSinceStartup)
            {
                return true;
            }
            return false;
        }
    }

}
