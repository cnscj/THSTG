using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;

namespace ASGame
{
    //缓冲数据信息
    public class AssetCacheObject : BaseAssetRef
    {
        public readonly Object cacheObj;        //缓冲物体
        public readonly string cacheName;       //缓冲数据名称
        public float stayTime = -1f;            //0引用最长驻留时间s

        private float m_startTick;              //进入缓冲区时间

        public AssetCacheObject(string name, Object obj)
        {
            cacheName = name;
            cacheObj = obj;
        }

        //刷新进入缓冲区时间
        public void UpdateTick()
        {
            m_startTick = Time.realtimeSinceStartup;
        }

        public bool CheckRemove()
        {
            if (stayTime > 0 && RefCount() <= 0)
            {
                if (m_startTick + stayTime <= Time.realtimeSinceStartup)
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

        protected override void OnRelease()
        {

        }
    }

}
