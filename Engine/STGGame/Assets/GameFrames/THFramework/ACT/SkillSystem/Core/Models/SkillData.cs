using System.Collections.Generic;

namespace THGame
{
    [System.Serializable]
    public class SkillData
    {
        public int version;

        public SkillInfo skillInfo;
        public SkillTimeline skillTimleline;
        public SkillCombo skillCombo;
    }

}
