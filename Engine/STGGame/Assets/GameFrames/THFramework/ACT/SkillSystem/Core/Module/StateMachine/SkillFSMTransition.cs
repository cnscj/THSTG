using System;

namespace THGame
{
    public class SkillFSMTransition
    {
		public SkillFSMState FromState { get; private set; }
		public SkillFSMState ToState { get; private set; }

		public event Action<SkillFSMTransition> OnComplete;
		public event Action<SkillFSMTransition> OnTransiting;

		private readonly Func<bool> testConditionFunc;

		public SkillFSMTransition(SkillFSMState from, SkillFSMState to, Func<bool> testConditionFunction = null)
		{
			FromState = from;
			ToState = to;
			testConditionFunc = testConditionFunction;
		}

		protected void Complete()
		{
			OnComplete?.Invoke(this);
		}

		public virtual void Begin()
		{
			Complete();
		}

		public bool TestCondition()
		{
			return testConditionFunc == null || testConditionFunc();
		}

	}

}
