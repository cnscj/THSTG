using UnityEngine;
using System.Collections;

namespace THGame
{
    public class SoundPoolObject : MonoBehaviour
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string key;

        /// <summary>
        /// 所属对象池
        /// </summary>
        public SoundPool poolObj;

        /// <summary>
        /// 对象显示的持续时间，若=0，则不隐藏
        /// </summary>
        public float lifetime;

        /// <summary>
        /// 池清空时的次数
        /// </summary>
        public int times;

        /// <summary>
        /// 在池中能存活的最长时间
        /// </summary>
        public float stayTime = 30f;

        /// <summary>
        /// 上次使用的tick
        /// </summary>
        private float m_updateTick;

        void OnEnable()
        {
            UpdateTick();

            if (!enabled)
                return;

            if (lifetime > 0)
            {
                StartCoroutine(CountDown());
            }
        }

        IEnumerator CountDown()
        {
            yield return new WaitForSeconds(lifetime);
            Release();
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

    }
}
