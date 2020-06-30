
using UnityEngine;
using Object = System.Object;

namespace ASGame
{
    //缓冲数据信息
    public class AssetObjectCacheObject : MonoBehaviour
    {
        public string cacheName;       //所属缓存数据名称
        public string cacheKey;        //缓存数据名称
        public Object cacheObj;        //缓存物体

        public float stayTime = -1f;   //最长驻留时间s
        private float m_updateTick;    //最后一次使用时间

        public static AssetObjectCacheObject Create(string cache, string key, Object obj, Transform parent = null)
        {
            GameObject gobj = new GameObject(key);
            var cacheObject = gobj.AddComponent<AssetObjectCacheObject>();
            cacheObject.cacheName = cache;
            cacheObject.cacheKey = key;
            cacheObject.cacheObj = obj;
            if (parent != null)
            {
                gobj.transform.SetParent(parent, false);
            }

            return cacheObject;
        }

        //刷新进入缓冲区时间
        public void UpdateTick()
        {
            m_updateTick = Time.realtimeSinceStartup;
        }

        public bool CheckRemove()
        {
            return IsExpired() || IsInvalid();
        }

        //过期的
        public bool IsExpired()
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

        //无效的
        public bool IsInvalid()
        {
            //如果cacheObj是UnityEngine.Object类型,这里判断为"null",即为false,导致无法及时移除
            return cacheObj == null;
        }

        public T GetObject<T>()
        {
            return (T)cacheObj;
        }

    }

}
