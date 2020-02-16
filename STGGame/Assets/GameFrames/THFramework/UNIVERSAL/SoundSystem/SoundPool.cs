using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace THGame
{
    public class SoundPool : MonoBehaviour
    {

        public long stayTime;

        private GameObject m_poolGObj;
        private Dictionary<string, Queue<SoundPlayer>> m_idleMap;
        public void Add(string key, SoundPlayer player)
        {

        }
        public void Get(string key)
        {
            //TODO:
        }

        public void Release(GameObject go)
        {
            if (go != null)
            {
                var soundPoolObj = go.GetComponent<SoundPoolObject>();
                if (soundPoolObj)
                {
                    Release(soundPoolObj);
                }
            }
        }
        public void Release(SoundPoolObject obj)
        {
            if (obj != null)
            {

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
