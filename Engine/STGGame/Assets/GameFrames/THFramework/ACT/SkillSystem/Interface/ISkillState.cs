using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public interface ISkillState
    {
        void StateChange(SkillFSMState fromState, SkillFSMState toState);
        void StateEnter(SkillFSMState state);
        void StateExit(SkillFSMState state);
    }

}
