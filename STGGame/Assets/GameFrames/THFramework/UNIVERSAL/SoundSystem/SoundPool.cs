using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace THGame
{
    public class SoundPool : MonoBehaviour
    {
        public long stayTime = -1;

        private int m_disposeTimes = 0;
        private Dictionary<string, Queue<SoundController>> m_idleMap;
        
        public SoundController GetOrCreate(string key)
        {
            Queue<SoundController> queue = null;
            SoundController ctrl = null;
            GameObject ctrlGobj = null;
            m_idleMap = m_idleMap ?? new Dictionary<string, Queue<SoundController>>();
            if (!m_idleMap.TryGetValue(key, out queue))
            {
                queue = new Queue<SoundController>();
                m_idleMap.Add(key, queue);
            }

            if (queue.Count <= 0)
            {
                ctrlGobj = new GameObject();
                ctrl = ctrlGobj.AddComponent<SoundController>();
                ctrlGobj.transform.SetParent(transform);

                queue.Enqueue(ctrl);
            }
      
            ctrl = queue.Dequeue();
            ctrlGobj = ctrl.gameObject;
            var poolObj = ctrlGobj.GetComponent<SoundPoolObject>();
            if (poolObj == null)
            {
                poolObj = ctrlGobj.AddComponent<SoundPoolObject>();
            }
            poolObj.times = m_disposeTimes;
            poolObj.poolObj = this;
            poolObj.key = key;

            ctrlGobj.SetActive(true);

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
                        Queue<SoundController> queue = null;
                        if (!m_idleMap.TryGetValue(key, out queue))
                        {
                            queue = new Queue<SoundController>();
                            m_idleMap.Add(key, queue);
                        }
                        queue.Enqueue(soundCtrl);

                        gobj.transform.SetParent(transform);
                        gobj.SetActive(false);
                    }
                }else
                {
                    Destroy(gobj);
                    return;
                }

                if (soundCtrl != null)
                {
                    
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
            if (m_idleMap != null)
            {
                foreach(var pair in m_idleMap)
                {
                    foreach(var ctrl in pair.Value)
                    {
                        Object.Destroy(ctrl.gameObject);
                    }

                }
            }
            m_idleMap.Clear();
            m_disposeTimes++;
        }
    }
}
