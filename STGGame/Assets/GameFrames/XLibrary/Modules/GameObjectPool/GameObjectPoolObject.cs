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
        public float lifetime = 0f;
        /// <summary>
        /// 所属对象池的唯一id
        /// </summary>
        public string poolName;

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
            GameObjectPoolManager.GetInstance().ReleaseGameObject(gameObject);
        }
    }


}
