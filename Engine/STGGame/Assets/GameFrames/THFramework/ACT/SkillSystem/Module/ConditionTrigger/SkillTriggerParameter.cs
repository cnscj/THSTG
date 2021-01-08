using System;
using System.Collections;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillTriggerParameter
    {
        public SkillTriggableParameterType type;
        public IComparable value;

        public void SetValue<T>(T newVal)
        {
            value = (IComparable)newVal;
        }

        public void SetValue(float newVal)
        {
            switch(type)
            {
                case SkillTriggableParameterType.Int:
                    value = (int)newVal;
                    break;
            }
        }
    }

}
