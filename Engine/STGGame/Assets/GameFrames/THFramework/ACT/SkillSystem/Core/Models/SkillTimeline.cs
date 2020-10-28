using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace THGame
{
    [System.Serializable]
    public class SkillTimeline
    {
        //一个技能的施法过程包括前摇,施法,后摇,而且施法可能有多个
        public SkillAction[] skillActions;
        public SkillEffect[] skillEffects;
        public SkillAudio[] skillAudios;
        public SkillEvent[] skillEvents;
    }
}