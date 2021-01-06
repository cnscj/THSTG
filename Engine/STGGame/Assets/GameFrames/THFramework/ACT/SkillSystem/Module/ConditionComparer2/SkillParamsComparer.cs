using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillParamComparer
    {
        private Dictionary<string, SkillConditionParam> _paramDict;

        public void AddParam(string key, SkillConditionParamType type)
        {

        }

        public void RemoveParam(string key)
        {

        }

        public void SetInt(string key,int value)
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

        private void SetNumber<T>(string key,T value)
        {

        }

        private Dictionary<string, SkillConditionParam> GetParamDict()
        {
            _paramDict = _paramDict ?? new Dictionary<string, SkillConditionParam>();
            return _paramDict;
        }
    }

}
