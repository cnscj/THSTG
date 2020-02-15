using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace THGame
{
    public class SoundPool : MonoBehaviour
    {
        public long stayTime;

        private GameObject m_pooGObj;
        private Dictionary<string, Queue<SoundPlayer>> m_idleMap;
        public void Get(string key)
        {
            //TODO:
        }

        public void Release(GameObject go)
        {
            if (go != null)
            {
                var soundpoolObj = go.GetComponent<SoundPoolObject>();
                if (soundpoolObj)
                {
                    Release(soundpoolObj);
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
    }
}
