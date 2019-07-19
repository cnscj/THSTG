using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace THGame
{
    public class BehaviourMapper : MonoBehaviour
    {
        //记录按键状态
        [System.Serializable]
        public class KeyPair
        {
            public int behaviour;
            public List<KeyCode> keycodes = new List<KeyCode>();
        }
        enum EKeyStatus
        {
            At = 2 ^ 0,
            Up = 2 ^ 1,
            Down = 2 ^ 2,
        }
        public List<KeyPair> keyList = new List<KeyPair>();
        public Dictionary<KeyCode, int> keyMaps = new Dictionary<KeyCode, int>();                       //按键映射
        public Dictionary<int, short> keyStatus = new Dictionary<int, short>();                         //按键状态

        public bool IsAtBehaviour(int behaviour)
        {
            if (keyStatus.ContainsKey(behaviour))
            {
                short status = keyStatus[behaviour];
                return ((status & (int)EKeyStatus.At) > 0);
            }
            return false;
        }

        public bool IsBehaving(int behaviour)
        {
            if (keyStatus.ContainsKey(behaviour))
            {
                short status = keyStatus[behaviour];
                return ((status & (int)EKeyStatus.Down) > 0);
            }
            return false;
        }

        public bool IsBehaved(int behaviour)
        {
            if (keyStatus.ContainsKey(behaviour))
            {
                short status = keyStatus[behaviour];
                return ((status & (int)EKeyStatus.Up) > 0);
            }
            return false;
        }

        private void Awake()
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
                short status = 0x0;
                bool []rets = new bool[3];
                foreach (var keyCode in keyPair.keycodes)
                {
                    rets[0] = rets[0] | Input.GetKey(keyCode);
                    rets[1] = rets[1] | Input.GetKeyDown(keyCode);
                    rets[2] = rets[2] | Input.GetKeyUp(keyCode);
                }
                status = rets[0] ? (short)(status | ((int)EKeyStatus.At)): status;
                status = rets[1] ? (short)(status | ((int)EKeyStatus.Down)) : status;
                status = rets[2] ? (short)(status | ((int)EKeyStatus.Up)) : status;
                keyStatus[keyPair.behaviour] = status;
            }
        }
    }

}
