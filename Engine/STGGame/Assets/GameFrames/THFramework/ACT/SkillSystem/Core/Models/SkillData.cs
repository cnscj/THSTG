using System;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    //某一系列技能的集合,每个技能互相独立
    [System.Serializable]
    public class SkillData : ScriptableObject
    {
        public int version;
        public SkillInfo[] skillInfos;
        public SkillCombo[] skillCombos;

        [NonSerialized] private Dictionary<int, SkillInfo> skillInfosDict;
        [NonSerialized] private Dictionary<int, SkillCombo> skillCombosDict;

        public Dictionary<int, SkillInfo> GetInfoDict()
        {
            if (skillInfosDict == null)
            {
                skillInfosDict = new Dictionary<int, SkillInfo>();
                if (skillInfos != null && skillInfos.Length > 0)
                {
                    foreach (var skillInfo in skillInfos)
                    {
                        skillInfosDict[skillInfo.skillId] = skillInfo;
                    }
                }
            }
            return skillInfosDict;
        }

        public Dictionary<int, SkillCombo> GetCombosDict()
        {
            if (skillCombosDict == null)
            {
                skillCombosDict = new Dictionary<int, SkillCombo>();
                if (skillCombos != null && skillCombos.Length > 0)
                {
                    foreach (var skillCombo in skillCombos)
                    {
                        skillCombosDict[skillCombo.skillId] = skillCombo;
                    }
                }
            }
            return skillCombosDict;
        }
    }

}
