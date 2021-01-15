
using System;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillBaseInfo
    {
        public string skillName;
        public string skillIcon;
        public string skillExStr;

        public float interruptedPrec = 20;          //抗打断能力
    }

}
