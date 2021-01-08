using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillTriggerParameters
    {
        public class KeyVlaue : ScriptableObject
        {
            public string key;
            public SkillTriggerParameter param;
        }

        public List<KeyVlaue> list;
        private Dictionary<string, SkillTriggerParameter> _paramsDict;

        public void AddParam(string key, SkillTriggerParameter param)
        {
            var newKeyValue = new KeyVlaue();
            newKeyValue.key = key;
            newKeyValue.param = param;

            list = list ?? new List<KeyVlaue>();
            list.Add(newKeyValue);
        }

        public void RemoveParam(string key)
        {
            if (list == null || list.Count <= 0)
                return;

            list.Remove(list.Where(t => t.key == key).FirstOrDefault());
        }

        public SkillTriggerParameter GetParam(string key)
        {
            var dict = GetParamsDict();
            dict.TryGetValue(key, out var param);
            return param;
        }

        private Dictionary<string, SkillTriggerParameter> GetParamsDict()
        {
            if (_paramsDict == null)
            {
                _paramsDict = _paramsDict ?? new Dictionary<string, SkillTriggerParameter>();
                if (list != null && list.Count > 0)
                {
                    foreach(var keyValue in list)
                    {
                        _paramsDict[keyValue.key] = keyValue.param;
                    }
                }
            }
            return _paramsDict;
        }
       
    }

}
