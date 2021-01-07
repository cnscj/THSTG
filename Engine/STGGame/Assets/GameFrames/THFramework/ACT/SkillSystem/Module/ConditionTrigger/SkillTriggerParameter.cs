using System;
using System.Collections;

namespace THGame
{
    [System.Serializable]
    public class SkillTriggerParameter
    {
        public SkillTriggableParameterType type;
        public IComparable value;
    }

}
