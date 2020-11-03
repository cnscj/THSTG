using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillItem
    {
        public string skillName;    //可以是Id
        public SkillInfo skillInfo;
        public SkillSequence skillSequence;
    }
}
