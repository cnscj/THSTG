using System.Collections;
using System.Collections.Generic;

namespace THGame
{
    [System.Serializable]
    public class SkillItem
    {
        public int skillId;
        public string skillName;
        public SkillAction skillAction;
        public SkillEffect skillEffect;
        public SkillAudio skillAudio;

        public SkillEvent skillEvent;
    }

}
