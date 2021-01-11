using System;
using System.Collections;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillTriggerParameter
    {
        public SkillTriggableParameterType type;
        [NonSerialized]public float value;

        public void SetInt(int newVal)
        {
            value = (int)newVal;
        }

        public void SetFloat(float newVal)
        {
            value = newVal;
        }

        public void SetBool(bool newVal)
        {
            value = newVal ? 1f : 0f;
        }
    }

}
