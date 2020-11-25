
using System;

namespace THGame
{
    [System.Serializable]
    public class SkillInfo
    {
        public int skillId;             //Id
        public string skillName;        //技能名

        public float cdTime;            //一次冷却CD时长
        public float maxTimes;          //最大使用次数
        public bool maxDuration;        //技能最大持续时间,时间到将进入冷却

        public bool canBeInterrupted;   //可被中断
        public int priority;            //优先级
        
        public string skillExStr;

    }

}
