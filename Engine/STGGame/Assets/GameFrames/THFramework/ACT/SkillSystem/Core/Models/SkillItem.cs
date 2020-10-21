
using System;

namespace THGame
{
    [System.Serializable]
    public class SkillItem
    {
        public int skillId;

        public float cdTime;
        public int invalidFrame;
        public bool canBeInterrupted;
        public string skillExStr;
        public Tuple<string, int>[] preconditions;

        public SkillAction skillAction;
        public SkillEffect skillEffect;
        public SkillAudio skillAudio;
        public SkillEvent skillEvent;
    }

}
