using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace THGame
{
    public class SoundPool : MonoBehaviour
    {

        public long stayTime;

        private GameObject m_poolGObj;
        private Dictionary<string, SoundController> m_idleMap;
        
        public SoundController GetOrCreate(string key)
        {
            SoundController ctrl = null;
            GameObject ctrlGobj = null;
            if (!m_idleMap.TryGetValue(key, out ctrl))
            {
                ctrlGobj = new GameObject(key);
                ctrl = ctrlGobj.AddComponent<SoundController>();

                m_idleMap.Add(key, ctrl);
            }

            //定时清理脚本
            ctrlGobj = ctrl.gameObject;
            var poolObj = ctrlGobj.GetComponent<SoundPoolObject>();
            if (poolObj == null)
            {
                poolObj = ctrlGobj.AddComponent<SoundPoolObject>();
            }
            ctrlGobj.SetActive(true);

            return ctrl;
        }

        public void Release(SoundPoolObject poolObj)
        {
            if (poolObj != null)
            {

                GameObject ctrlGobj = poolObj.gameObject;
                ctrlGobj.SetActive(false);
            }
        }

        public void Release(GameObject gobj)
        {
            if (gobj != null)
            {
                var poolObj = gobj.GetComponent<SoundPoolObject>();
                if (poolObj != null)
                {
                    Release(poolObj);
                }
                else
                {
                    Destroy(poolObj);
                }

            }
        }

        public void Release(SoundController ctrl)
        {
            if (ctrl != null)
            {
                Release(gameObject);
            }
        }

        private void Start()
        {
            
        }

        private Queue GetAvailableQueue(string key)
        {

            return null;
        }


    }
}
