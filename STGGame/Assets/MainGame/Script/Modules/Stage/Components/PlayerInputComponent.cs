
using System.Collections.Generic;
using UnityEngine;
namespace STGGame
{
    public class PlayerInputComponent : MonoBehaviour
    {
        //记录按键状态
        [System.Serializable]
        public class KeyPair
        {
            public EPlayerBehavior behaviour;
            public List<KeyCode> keycodes = new List<KeyCode>(); 
        }
        public List<KeyPair> keyList = new List<KeyPair>();
        public Dictionary<KeyCode, EPlayerBehavior> keyMaps = new Dictionary<KeyCode, EPlayerBehavior>();                   //按键映射
        public Dictionary<EPlayerBehavior, bool> keyStatus = new Dictionary<EPlayerBehavior, bool>();                       //按键状态

        //FIXME:不应该放到这里来
        private void Start()
        {
            foreach(var pair in keyList)
            {
                foreach(var keycode in pair.keycodes)
                {
                    if (!keyMaps.ContainsKey(keycode))
                    {
                        keyMaps.Add(keycode,pair.behaviour);
                    }
                }
            }
        }
    }

}
