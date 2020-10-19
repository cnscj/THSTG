using System.Collections;
using System.Collections.Generic;

namespace THGame
{
    [System.Serializable]
    public class SkillItem
    {
        public string skillName;
        public string skillDesc;

        //TODO:一个技能可能由好几段构成(起手前摇+释放)
        //TODO:一个技能可能由好几个动作构成
        public SkillAction skillAction;
        public SkillEffect skillEffect;
        public SkillAudio skillAudio;

        public SkillEvent skillEvent;


    }

}
