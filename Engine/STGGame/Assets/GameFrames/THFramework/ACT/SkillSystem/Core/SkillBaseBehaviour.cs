using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public abstract class SkillBaseBehaviour
    {
        protected virtual void OnChanged(SkillFSMState fromState, SkillFSMState toState)
        {

        }

        protected virtual void OnEnter(SkillFSMState state)
        {

        }

        protected virtual void OnExit(SkillFSMState state)
        {

        }
    }
}

