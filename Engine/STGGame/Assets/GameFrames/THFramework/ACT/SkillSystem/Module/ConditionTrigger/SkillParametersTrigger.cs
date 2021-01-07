using System.Collections;
using System.Collections.Generic;

namespace THGame
{
    public class SkillParametersTrigger
    {
        //触发器类型的,判断完所有后下一帧直接置false
        public SkillTriggerParameters parameters;

        public void SetInt(string key, int value)
        {
            SetNumber(key, value);
        }

        public void SetFloat(string key, float value)
        {
            SetNumber(key, value);
        }

        public void SetBool(string key, bool value)
        {
            SetNumber(key, value);
        }

        public void Trigge(string key)
        {

        }

        public bool Verify(SkillTriggableConditions conditions)
        {
            if (conditions == null)
                return false;
            if (conditions.list == null || conditions.list.Count <= 0)
                return false;

            foreach(var condition in conditions.list)
            {
                var param = parameters.GetParam(condition.key);

            }


            return false;
        }

        private void SetNumber<T>(string key, T value)
        {

        }
    }


    
}
