using System;
using UnityEngine;

namespace THGame
{
    public abstract class SkillBaseBehaviour : MonoBehaviour
    {
        public Action<SkillFSMState, SkillFSMState> onStateChanged;
        private SkillFSMMachine _skillMachine;

        public void BuildStateMachine(SkillBean[] skillBeans)
        {
            if (skillBeans == null || skillBeans.Length <= 0)
                return;

            //根据配置,构建对应的状态机
            _skillMachine = new SkillFSMMachine();
            _skillMachine.OnChange(StateChanged);
            _skillMachine.OnTransition(StateTransition);

            foreach (var skillBean in skillBeans)
            {

            }
        }

        private bool StateTransition(SkillFSMTransition transition)
        {
            return OnStateTransition(transition);
        }

        private void StateChanged(SkillFSMState fromState, SkillFSMState toState)
        {
            onStateChanged?.Invoke(fromState, toState);
            OnStateChanged(fromState, toState);
        }

        protected virtual bool OnStateTransition(SkillFSMTransition transition)
        {
            return true;
        }

        protected virtual void OnStateChanged(SkillFSMState fromState, SkillFSMState toState)
        {

        }

    }
}

