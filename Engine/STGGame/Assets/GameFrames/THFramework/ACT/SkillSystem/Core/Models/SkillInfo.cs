
using System;

namespace THGame
{
    [System.Serializable]
    public class SkillInfo
    {
        public int skillId;             //Id
        public string skillName;        //技能名

        //CD触发类型应该读配置(可能有技能结束冷却,可能有其他冷却
        public float cdTime;            //一次冷却CD时长(TODO:时长应该取读配置
        public float maxTimes;          //最大使用次数
        public bool maxDuration;        //技能最大持续时间,时间到将进入冷却

        public bool canBeInterrupted;   //可被中断
        public int priority;            //优先级
        
        public string skillExStr;

    }

}
