using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace THGame
{
    [System.Serializable]
    public class SkillTimeline
    {
        //一个技能的施法过程包括前摇,施法,后摇,而且施法可能有多个
        //冲刺,可以取消后摇
        //前摇受击会被打断技能,但也不一定被打断
        public SkillAction[] skillActions;
        public SkillEffect[] skillEffects;
        public SkillAudio[] skillAudios;
        public SkillEvent[] skillEvents;
    }
}