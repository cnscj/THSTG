using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public abstract class SkillBaseBehaviour
    {
        //创建一个状态机
        private SkillFSMMachine _skillMachine;

        public void BuildStateMachine()
        {
            //根据配置,构建对应的状态机
        }

        protected virtual void OnTransitive(SkillFSMTransition transition)
        {

        }

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

