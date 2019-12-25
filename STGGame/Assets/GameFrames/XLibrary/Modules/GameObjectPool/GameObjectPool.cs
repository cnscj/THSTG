using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public class GameObjectPool : MonoBehaviour
    {
        public enum ReleaseOperate
        {
            Active,         //失活
            Sight,          //视野
        }
        /// <summary>
        /// 每个对象池的名称，当唯一id
        /// </summary>
        public string poolName;
        /// <summary>
        /// 对象预设
        /// </summary>
        public GameObject prefab;
        /// <summary>
        /// 对象池中存放最大数量
        /// </summary>
        public int maxCount = 20;
        /// <summary>
        /// 默认初始容量
        /// </summary>
        public int defaultCount = 0;

        /// <summary>
        /// 满池时不会生成
        /// </summary>
        public bool fixedSize = false;

        /// <summary>
        ///释放模式
        /// </summary>
        public ReleaseOperate releaseOperate = ReleaseOperate.Active;

        /// <summary>
        /// 被托管
        /// </summary>
        public bool isManaged = true;

        /// <summary>
        /// 队列，存放对象池中没有用到的对象，即可分配对象
        /// </summary>
        protected Queue m_queue;


        public GameObjectPool()
        {
            m_queue = new Queue();
        }

        /// <summary>
        /// 生成一个对象
        /// </summary>
        /// <param name="lifetime">对象存在的时间</param>
        /// <returns>生成的对象</returns>
        public GameObject Create(float lifetime = 0f)
        {
            if (lifetime < 0)
            {
                //lifetime<0时，返回null  
                return null;
            }
            bool isPoolObj = false;
            GameObject returnObj;
            if (m_queue.Count > 0)
            {
                //池中有待分配对象
                returnObj = (GameObject)m_queue.Dequeue();
                isPoolObj = true;
            }
            else
            {
                if (prefab == null) return null;
                if (fixedSize) return null;
                //池中没有可分配对象了，新生成一个
                returnObj = Object.Instantiate(prefab) as GameObject;
                returnObj.transform.SetParent(gameObject.transform);
                returnObj.SetActive(false);
                
            }
            //使用PrefabInfo脚本保存returnObj的一些信息
            GameObjectPoolObject info = returnObj.GetComponent<GameObjectPoolObject>();
            if (info == null)
            {
                info = returnObj.AddComponent<GameObjectPoolObject>();
            }
            info.poolName = poolName;
            if (lifetime > 0)
            {
                info.lifetime = lifetime;
            }

            switch (releaseOperate)
            {
                case ReleaseOperate.Active:
                    returnObj.SetActive(true);
                    break;
                case ReleaseOperate.Sight:
                {
                    if (isPoolObj)
                    {
                        var position = returnObj.transform.position;
                        position.z -= -1000f;
                        returnObj.transform.position = position;
                    }
                    else
                    {
                        returnObj.SetActive(true);
                    }
            }
                break;

            }
            return returnObj;
        }

        /// <summary>
        /// “删除对象”放入对象池
        /// </summary>
        /// <param name="obj">对象</param>
        public void Release(GameObject obj)
        {
            //待分配对象已经在对象池中  
            if (m_queue.Contains(obj))
            {
                return;
            }
            if (m_queue.Count > maxCount)
            {
                //当前池中object数量已满，直接销毁
                Object.Destroy(obj);
            }
            else
            {
                //放入对象池，入队
                m_queue.Enqueue(obj);
               
                obj.transform.SetParent(transform, false); //不改变Transform
                switch (releaseOperate)
                {
                    case ReleaseOperate.Active:
                        obj.SetActive(false);
                        break;
                    case ReleaseOperate.Sight:
                        var position = obj.transform.position;
                        position.z += -1000f;
                        obj.transform.position = position;
                        break;

                }
            }
        }

        /// <summary>
        /// 销毁该对象池
        /// </summary>
        public void Destroy()
        {
            m_queue.Clear();
            Destroy(gameObject);
        }

        /// <summary>
        /// 根据池原有初始化
        /// </summary>
        public void Init()
        {
            for (int i = 0; i < defaultCount && i < maxCount; i++)
            {
                if (i < transform.childCount)
                {
                    GameObject availableGameObject = transform.GetChild(i).gameObject;
                    Release(availableGameObject);   //放回池中待利用
                }
                else
                {
                    if (prefab != null)
                    {
                        GameObject availableGameObject = Object.Instantiate(prefab) as GameObject;
                        Release(availableGameObject);   //放回池中待利用
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 将自己加入到对象池管理中去
        /// </summary>
        private void Awake()
        {
            if (isManaged)
            {
                GameObjectPoolManager.GetInstance().AddGameObjectPool(this);
            }
            else
            {
                Init();
            }

        }

        /// <summary>
        // 移除掉无效的自己
        /// </summary>
        private void Start()
        {
            if (string.IsNullOrEmpty(poolName) || (isManaged && !GameObjectPoolManager.GetInstance().GetGameObjectPool(poolName)))
            {
                Destroy();
            }
        }

        /// <summary>
        /// 被销毁清空自己
        /// </summary>
        private void OnDestroy()
        {
            var mgrInstance = GameObjectPoolManager.GetInstance();
            if (mgrInstance != null)
            {
                if (mgrInstance.GetGameObjectPool(poolName))
                {
                    mgrInstance.DestroyGameObjectPool(poolName);
                }
            }

        }
    }
}
