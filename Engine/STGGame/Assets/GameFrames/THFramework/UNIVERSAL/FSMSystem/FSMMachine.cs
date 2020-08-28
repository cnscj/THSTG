using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_WSA
using System.Reflection;
#endif
namespace THGame
{
	public class FSMMachine
	{
		public string Name { get; set; }
		public FSMState CurrentState { get; private set; }
		public FSMTransition CurrentTransition { get; private set; }

		private Dictionary<string, FSMState> _states = new Dictionary<string, FSMState>();
		private Dictionary<string, FSMTransition> _transitionsFrom = new Dictionary<string, FSMTransition>();
		private Dictionary<FSMTransition, string> _transitionsTo = new Dictionary<FSMTransition, string>();

		private event Action<FSMState> OnStateEnter;
		private event Action<FSMState> OnStateExit;
		private event Action<FSMState, FSMState> OnStateChange;

		public void SetDefalutState(string name)
        {
			if (string.IsNullOrEmpty(name)) return;
			if (_states.ContainsKey(name)) return;

			CurrentState = _states[name];
		}

		public FSMState AddState(string name)
        {
			if (string.IsNullOrEmpty(name)) return null;
			if (_states.ContainsKey(name)) return null;

			var state = new FSMState(name);
			return state;
        }

		public FSMTransition AddTransition(string from, string to, Func<bool> condition = null)
		{
			if (string.IsNullOrEmpty(from)) return null;
			if (string.IsNullOrEmpty(to)) return null;
			if (!_states.ContainsKey(from)) return null;
			if (!_states.ContainsKey(to)) return null;

			var transition = new FSMTransition(condition);
			_transitionsFrom.Add(from, transition);
			_transitionsTo.Add(transition, to);

			return transition;
		}

		//TODO:
		public bool Transfer(string stateName)
        {
			var transition = _transitionsFrom[stateName];
			if (transition.TestCondition())
			{
				transition.OnComplete += HandleTransitionComplete;
				CurrentTransition = transition;
				if (OnStateExit != null)
				{
					OnStateExit(CurrentState);
				}
				transition.Begin();
				return true;
			}
			return false;
		}

		private void HandleTransitionComplete()
		{
			CurrentTransition.OnComplete -= HandleTransitionComplete;

			var previousState = CurrentState;
			string toStateName = _transitionsTo[CurrentTransition];
			CurrentState = _states[toStateName];

			CurrentTransition = null;

			previousState.Exit();
			if (OnStateChange != null)
			{
				OnStateChange(previousState, CurrentState);
			}

			CurrentState.Enter();
			if (OnStateEnter != null)
			{
				OnStateEnter(CurrentState);
			}
		}

		public FSMMachine OnEnter(string state, Action handler)
		{
			if (string.IsNullOrEmpty(state)) { throw new ArgumentNullException("state"); }
			if (handler == null) { throw new ArgumentNullException("handler"); }
			if (!_states.ContainsKey(state)) { throw new ArgumentException("unknown state", "state"); }

			OnStateEnter += enteredState =>
			{
				if (enteredState.Equals(state))
				{
					handler();
				}
			};

			return this;
		}

		public FSMMachine OnExit(string state, Action handler)
		{
			if (string.IsNullOrEmpty(state)) { throw new ArgumentNullException("state"); }
			if (handler == null) { throw new ArgumentNullException("handler"); }
			if (!_states.ContainsKey(state)) { throw new ArgumentException("unknown state", "state"); }

			OnStateExit += exitedState =>
			{
				if (exitedState.Equals(state))
				{
					handler();
				}
			};
			return this;
		}

		public FSMMachine OnChange(Action<FSMState, FSMState> handler)
		{
			if (handler == null) { throw new ArgumentNullException("handler"); }

			OnStateChange += handler;

			return this;
		}

	
		public FSMMachine OnChange(string from, string to, Action handler)
		{
			if (string.IsNullOrEmpty(from)) { throw new ArgumentNullException("from"); }
			if (string.IsNullOrEmpty(to)) { throw new ArgumentNullException("to"); }
			if (!_states.ContainsKey(from)) { throw new ArgumentException("unknown state", "from"); }
			if (!_states.ContainsKey(to)) { throw new ArgumentException("unknown state", "to"); }
			if (handler == null) { throw new ArgumentNullException("handler"); }

			OnStateChange += (fromState, toState) =>
			{
				if (fromState.Equals(from) &&
					toState.Equals(to))
				{
					handler();
				}
			};

			return this;
		}
	}
}
