using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillEvent
    {
        public float timeTick;          //时间
        public string eventName;        //事件名
        public object args;             //相关参数
    }
}
