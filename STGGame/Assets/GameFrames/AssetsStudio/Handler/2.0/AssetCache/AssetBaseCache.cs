
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetBaseCache : MonoBehaviour
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