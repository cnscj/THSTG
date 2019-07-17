using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace THGame
{
    public class BehaviourMppper : MonoBehaviour
    {
        //记录按键状态
        [System.Serializable]
        public class KeyPair
        {
            public int behaviour;
            public List<KeyCode> keycodes = new List<KeyCode>();
        }
        public List<KeyPair> keyList = new List<KeyPair>();
        public Dictionary<KeyCode, int> keyMaps = new Dictionary<KeyCode, int>();                   //按键映射
        public Dictionary<int, bool> keyStatus = new Dictionary<int, bool>();                       //按键状态

        public bool IsAtBehaviour(int behaviour)
        {
            if (keyStatus.ContainsKey(behaviour))
            {
                return keyStatus[behaviour];
            }
            return false;
        }

        private void Start()
        {
            foreach (var pair in keyList)
            {
                foreach (var keycode in pair.keycodes)
                {
                    if (!keyMaps.ContainsKey(keycode))
                    {
                        keyMaps.Add(keycode, pair.behaviour);
                    }
                }
            }
        }

        private void Update()
        {
            //把所有的按键记录到输入组件中
            foreach (var keyPair in keyList)
            {
                bool ret = false;
                foreach (var keyCode in keyPair.keycodes)
                {
                    ret = ret | Input.GetKey(keyCode);
                }
                keyStatus[keyPair.behaviour] = ret;
            }
        }
    }

}
