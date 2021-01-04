using System;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    //某一系列技能的集合,每个技能互相独立
    [System.Serializable]
    [CreateAssetMenu(menuName = "THGame/SkillEditor/Create SkillData")]
    public partial class SkillData : ScriptableObject
    {
        public int version;
        public SkillBean[] skillBeans;
    }

}
