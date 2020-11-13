using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillBehaviour
    {
        //一个技能的施法过程包括前摇,施法,后摇,而且施法可能有多个
        //冲刺,可以取消后摇
        //有些后摇无法取消,要看优先级(比如冲刺100%取消后摇,但是如果是处于中间的连招,则无法取消后摇)
        //前摇受击会被打断技能,但也不一定被打断
        public bool canBeInterrupted;       //可被中断
        public float minTriggeringTime;     //最短触发时间
        public float maxTriggeringTime;     //最大触发时间

        public SkillTimeline[] skillTimelines;
    }
}
