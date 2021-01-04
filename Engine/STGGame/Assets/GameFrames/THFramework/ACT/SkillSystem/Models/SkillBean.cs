using UnityEngine;

namespace THGame
{
    //某一系列技能的集合,每个技能互相独立
    //AI也可以使用,因此不能与主角绑定
    [System.Serializable]
    public class SkillBean
    {
        public int skillId;
        public SkillType skillType;
        public SkillBaseInfo skillBaseInfo;
        public SkillCastInfo skillCastInfo;
        public SkillActionInfo skillActionInfo;
    }

}
