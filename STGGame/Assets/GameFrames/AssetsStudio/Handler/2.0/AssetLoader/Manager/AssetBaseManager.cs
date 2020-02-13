
using UnityEngine;
using XLibrary.Package;

namespace ASGame
{
    /// <summary>
    /// 一个简易对象池及轮询系统
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AssetBaseManager<T> : MonoSingleton<T> where T: MonoBehaviour
    {
        [Header("定时清理缓存间隔(秒):")]
        public float clearCacheDuration = 10f;

        private float m_cacheTimeTemp;

        private void Update()
        {
            OnUpdate();

            if (clearCacheDuration < 0) return;
            m_cacheTimeTemp += Time.deltaTime;
            if (m_cacheTimeTemp >= clearCacheDuration)
            {
                OnClean();
                m_cacheTimeTemp -= clearCacheDuration;
            }
        }

        protected void OnUpdate()
        {

        }

        protected void OnClean()
        {

        }
    }
}