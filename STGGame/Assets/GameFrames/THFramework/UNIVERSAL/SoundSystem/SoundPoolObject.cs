using UnityEngine;
using System.Collections;

namespace THGame
{
    public class SoundPoolObject : MonoBehaviour
    {
        /// <summary>
        /// 对象显示的持续时间，若=0，则不隐藏
        /// </summary>
        public float lifetime = 0f;

        /// <summary>
        /// 所属对象池
        /// </summary>
        public SoundPool poolObj;

        void OnEnable()
        {
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

    }
}
