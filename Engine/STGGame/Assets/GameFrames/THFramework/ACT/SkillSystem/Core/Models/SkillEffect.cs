﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    //有可能是模型特效,也有可能是普通特效
    [System.Serializable]
    public class SkillEffect
    {
        public string effectKey;                //特效节点
        public int maxCount = -1;               //最大同特效数量
        public float liveTime = -1;             //生存时间

    }

}
