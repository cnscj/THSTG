using System;

namespace XLibGame
{
	/// <summary>
	/// Controls a transition from a FromState to a ToState.
	/// </summary>
	/// <typeparam name="TState"></typeparam>
	public abstract class FSMTransition<TState> where TState : IComparable
	{
		public TState FromState { get; private set; }
		public TState ToState { get; private set; }

		private readonly Func<bool> testConditionFunc;

		public event Action OnComplete;

		protected FSMTransition(TState from, TState to, Func<bool> testConditionFunction = null)
		{
			FromState = from;
			ToState = to;
			testConditionFunc = testConditionFunction;
		}

		protected void Complete()
		{
			if (OnComplete != null) OnComplete();
		}

		public abstract void Begin();

		public bool TestCondition()
		{
			return testConditionFunc == null || testConditionFunc();
		}
	}

	public class FSMDefaultStateTransition<TState> : FSMTransition<TState> where TState : IComparable
	{
		public FSMDefaultStateTransition(TState from, TState to, Func<bool> testConditionFunction = null) 
			: base(from, to, testConditionFunction)
		{
		}

		public override void Begin()
		{
			Complete();
		}
	}
}