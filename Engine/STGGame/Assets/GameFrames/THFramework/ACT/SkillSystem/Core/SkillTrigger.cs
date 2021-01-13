using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillTrigger : MonoBehaviour
    {
        private Dictionary<IComparable, SkillBaseHandler> _skillHandlerDict;

        public void GenStateMachine(SkillData skillData)
        {

        }

        public void Cast(IComparable state)
        {
 
        }

        public SkillBaseHandler GetHandler(IComparable key)
        {
            if (_skillHandlerDict == null || _skillHandlerDict.Count <= 0)
                return default;

            _skillHandlerDict.TryGetValue(key, out var handler);
            return handler;
        }

        private Dictionary<IComparable, SkillBaseHandler> GetHandlerDict()
        {
            _skillHandlerDict = _skillHandlerDict ?? new Dictionary<IComparable, SkillBaseHandler>();
            return _skillHandlerDict;
        }
    }
}
