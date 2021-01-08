using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillTriggableConditions
    {
        [System.Serializable]
        public class KeyValue
        {
            public string key;
            public SkillTriggableCondition condition;
        }
        public List<KeyValue> list;

        public void AddCondition(string key, SkillTriggableCondition condition)
        {
            var keyValue = new KeyValue();
            keyValue.key = key;
            keyValue.condition = condition;

            list = list ?? new List<KeyValue>();
            list.Add(keyValue);
        }

        public void RemoveCondition(string key)
        {
            if (list == null || list.Count <= 0)
                return;

            list.Remove(list.Where(t => t.key == key).FirstOrDefault());
        }
    }

}
