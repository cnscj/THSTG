using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public abstract class AbstractSkillTrigger : SkillTimelineTrack
    {
        public AbstractSkillTrigger(float startTime = 0,float durationTime = -1) : base(startTime, durationTime) { }
        public string Type { get; set; }
    }

}
