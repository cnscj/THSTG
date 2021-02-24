using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public abstract class AbstractSkillTrigger : SkillTimelineClip
    {
        public AbstractSkillTrigger() { }
        public string Type { get; set; }
    }

}
