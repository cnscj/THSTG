using System;

namespace THGame
{
    public class FSMTransition
    {
		public FSMState FromState { get; private set; }
		public FSMState ToState { get; private set; }

		public event Action<FSMTransition> OnComplete;
		public event Action<FSMTransition> OnTransiting;

		private readonly Func<bool> testConditionFunc;

		public FSMTransition(FSMState from, FSMState to, Func<bool> testConditionFunction = null)
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