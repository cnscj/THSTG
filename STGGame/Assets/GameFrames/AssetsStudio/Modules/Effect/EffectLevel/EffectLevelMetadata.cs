﻿using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    [System.Serializable]
    public class EffectLevelMetadata
    {
        [SerializeField]
        public int level;                               //等级
        [SerializeField]
        public List<string> effectList;             //节点信息
    }
}