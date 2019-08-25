using UnityEngine;
using Object = UnityEngine.Object;

namespace THGame
{
    //缓冲数据信息
    public class ResourceCacheDataInfo
    {
        public float startTick;     //进入缓冲区时间
        public Object cacheObj;     //缓冲物体
        public string cacheName;    //缓冲数据名称

        public ResourceCacheDataInfo(string name,Object obj)
        {
            cacheName = name;
            cacheObj = obj;
        }

        //刷新进入缓冲区时间
        public void UpdateTick()
        {
            startTick = Time.realtimeSinceStartup;
        }
    }

}
