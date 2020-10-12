using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillEffect
    {
        public string effectKey;                //特效节点
        public int maxCount = -1;                //最大同特效数量
        public float liveTime = -1;             //生存时间

    }

}
