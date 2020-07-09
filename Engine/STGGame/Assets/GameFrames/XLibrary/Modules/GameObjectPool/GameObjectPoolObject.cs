using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public class GameObjectPoolObject : MonoBehaviour
    {
        /// <summary>
        /// 对象显示的持续时间，若=0，则不隐藏
        /// </summary>
        public float lifeTime;

        /// <summary>
        /// 与Dispose挂钩
        /// </summary>
        public int postTimes;

        /// <summary>
        /// 在池中的存活时间
        /// </summary>
        public float stayTime;

        /// <summary>
        /// 所属对象池
        /// </summary>
        public GameObjectPool poolObj;

        private float m_stayTick;

        void OnEnable()
        {
            if (!enabled)
                return;

            if (lifeTime > 0)
            {
                StartCoroutine(CountDown());
            }
        }

        public bool CheckTick()
        {
            if (stayTime > 0)
            {
                if (Time.realtimeSinceStartup - m_stayTick >= stayTime)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateTick()
        {
            m_stayTick = Time.realtimeSinceStartup;
        }

        public void Release()
        {
            //将对象加入对象池
            if (poolObj != null)
            {
                poolObj.Release(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        IEnumerator CountDown()
        {
            yield return new WaitForSeconds(lifeTime);
            Release();
        }
    }


}
