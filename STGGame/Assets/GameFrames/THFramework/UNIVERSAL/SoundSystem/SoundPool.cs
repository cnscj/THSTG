using UnityEngine;
using System.Collections.Generic;

namespace THGame
{
    public class SoundPool : MonoBehaviour
    {
        public class PoolInfo
        {
            public float stayTime;
            public Stack<SoundController> idleQueue = new Stack<SoundController>();//用栈使得空闲的频繁使用,

            public int curMaxCount;
            public float updateTick;
        }

        private int m_disposeTimes = 0;
        private Dictionary<string, PoolInfo> m_poolMap = new Dictionary<string, PoolInfo>();
        private Stack<string> m_releaseList = new Stack<string>();      
        
        public SoundController GetOrCreate(string key,float lifeTime = 0f)
        {
            PoolInfo poolInfo = null;
            SoundController ctrl = null;
            GameObject ctrlGobj = null;
            if (!m_poolMap.TryGetValue(key, out poolInfo))
            {
                poolInfo = new PoolInfo();
                m_poolMap.Add(key, poolInfo);
            }

            if (poolInfo.idleQueue.Count <= 0)
            {
                ctrlGobj = new GameObject();
                ctrl = ctrlGobj.AddComponent<SoundController>();
                ctrlGobj.transform.SetParent(transform);

                poolInfo.idleQueue.Push(ctrl);
            }
      
            ctrl = poolInfo.idleQueue.Pop();
            ctrlGobj = ctrl.gameObject;
            var poolObj = ctrlGobj.GetComponent<SoundPoolObject>();
            if (poolObj == null)
            {
                poolObj = ctrlGobj.AddComponent<SoundPoolObject>();
            }
            poolObj.lifetime = lifeTime;
            poolObj.times = m_disposeTimes;
            poolObj.poolObj = this;
            poolObj.key = key;

            ctrlGobj.SetActive(true);

            UpdatePoolInfo(key);

            return ctrl;
        }

        public void Release(SoundPoolObject poolObj)
        {
            if (poolObj != null)
            {
                Release(poolObj.gameObject);
            }
            else
            {
                Destroy(poolObj.gameObject);
            }
        }

        public void Release(GameObject gobj)
        {
            if (gobj != null)
            {
                var poolObj = gobj.GetComponent<SoundPoolObject>();
                var soundCtrl = gobj.GetComponent<SoundController>();
                    
                if (poolObj != null)
                {
                    if (poolObj.times < m_disposeTimes)
                    {
                        Destroy(poolObj.gameObject);
                        return;
                    }
                    else
                    {
                        string key = poolObj.key;
                        PoolInfo poolInfo = null;
                        if (!m_poolMap.TryGetValue(key, out poolInfo))
                        {
                            poolInfo = new PoolInfo();
                            m_poolMap.Add(key, poolInfo);
                        }
                        poolInfo.idleQueue.Push(soundCtrl);

                        gobj.transform.SetParent(transform);
                        gobj.SetActive(false);

                        UpdatePoolInfo(key);
                    }
                }
                else
                {
                    Destroy(gobj);
                    return;
                }
            }
        }

        public void Release(SoundController ctrl)
        {
            if (ctrl != null)
            {
                Release(ctrl.gameObject);
            }
            else
            {
                Destroy(ctrl.gameObject);
            }
        }

        public void Dispose()
        {
            foreach(var pair in m_poolMap)
            {
                var idleQueue = pair.Value.idleQueue;
                foreach (var ctrl in idleQueue)
                {
                    Object.Destroy(ctrl.gameObject);
                }
                idleQueue.Clear();
            }
            m_poolMap.Clear();
            m_disposeTimes++;
        }

        private void UpdatePoolInfo(string poolName)
        {
            if (m_poolMap.TryGetValue(poolName,out var poolInfo))
            {
                poolInfo.updateTick = Time.realtimeSinceStartup;
                poolInfo.curMaxCount = Mathf.Max(poolInfo.curMaxCount, poolInfo.idleQueue.Count);
            }
        }

        private void Update()
        {
            foreach (var pair in m_poolMap)
            {

                var poolInfo = pair.Value;
                if (poolInfo.stayTime > 0f)
                {
                    if (poolInfo.idleQueue.Count >= poolInfo.curMaxCount)
                    {
                        if (poolInfo.updateTick + poolInfo.stayTime <= Time.realtimeSinceStartup)
                        {
                            var poolKey = pair.Key;
                            m_releaseList.Push(poolKey);
                        }
                    }
                    else
                    {
                        poolInfo.updateTick = Time.realtimeSinceStartup;
                    }

                }
            }
            while(m_releaseList.Count > 0 )
            {
                var poolKey = m_releaseList.Pop();
                if (m_poolMap.TryGetValue(poolKey,out var poolInfo))
                {
                    var idleQueue = poolInfo.idleQueue;
                    foreach (var ctrl in idleQueue)
                    {
                        Object.Destroy(ctrl.gameObject);
                    }
                    idleQueue.Clear();
                }
                m_poolMap.Remove(poolKey);
            } 
        }
    }
}
