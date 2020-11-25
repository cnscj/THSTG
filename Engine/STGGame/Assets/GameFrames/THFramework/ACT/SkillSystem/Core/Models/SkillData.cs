using System.Collections.Generic;

namespace THGame
{
    [System.Serializable]
    public class SkillData
    {
        public int version;

        public SkillInfo[] skillInfos;
        public SkillCombo[] skillCombos;
    }

}
