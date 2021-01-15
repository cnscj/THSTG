using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillTrigger : MonoBehaviour
    {
        private Dictionary<IComparable, SkillBehaviour> _skillBehaviourDict;

        public void GenStateMachine(SkillData skillData)
        {

        }

        public void CastKeyUp(IComparable state)
        {
 
        }

        public void CastKeyDown(IComparable state)
        {

        }

        public SkillBehaviour GetHandler(IComparable key)
        {
            if (_skillBehaviourDict == null || _skillBehaviourDict.Count <= 0)
                return default;

            _skillBehaviourDict.TryGetValue(key, out var handler);
            return handler;
        }

        private Dictionary<IComparable, SkillBehaviour> GetBehaviourDict()
        {
            _skillBehaviourDict = _skillBehaviourDict ?? new Dictionary<IComparable, SkillBehaviour>();
            return _skillBehaviourDict;
        }
    }
}
