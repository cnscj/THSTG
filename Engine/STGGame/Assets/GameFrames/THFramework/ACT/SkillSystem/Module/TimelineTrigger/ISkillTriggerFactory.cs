using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public interface ISkillTriggerFactory
    {
        string Type { get; set; }
        AbstractSkillTrigger CreateTrigger();
        void RecycleTrigger(AbstractSkillTrigger instance);
    }
}
