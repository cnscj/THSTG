using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillCombo
    {
        public int skillId;
        public SkillCastType castType;              //技能类型
        public SkillInputType inputType;            //输入类型
        public float minMagicTime;                  //最小施法时间--如果操作大于这个时间,开始施法(只针对长按有效
        public float maxMagicTime;                  //最大施法时间--如果操作大于这个时间,就中断施法

    }

}
