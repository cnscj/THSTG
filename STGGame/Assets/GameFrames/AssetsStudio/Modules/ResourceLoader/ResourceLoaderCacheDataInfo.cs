using UnityEngine;
using Object = UnityEngine.Object;

namespace ASGame
{
    //缓冲数据信息
    public class ResourceLoaderCacheDataInfo
    {
        public Object cacheObj;     //缓冲物体
        public string cacheName;    //缓冲数据名称
        public float stayTime;      //驻留时间

        public float startTick;     //进入缓冲区时间

        public ResourceLoaderCacheDataInfo(string name,Object obj,float time = 30f)
        {
            cacheName = name;
            cacheObj = obj;
            stayTime = time;
        }

        //刷新进入缓冲区时间
        public void UpdateTick()
        {
            startTick = Time.realtimeSinceStartup;
        }
    }

}
