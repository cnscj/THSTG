
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillEvent : BaseSkillTrack
    {
        public string eventName;                    //事件名
        public string args1;                        //相关参数1
        public GameObject args2;                    //相关参数2
    }
}
