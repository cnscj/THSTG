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

		public FSMState PreviousState { get; private set; }
		public FSMState CurrentState { get; private set; }
		public FSMTransition CurrentTransition { get; private set; }

		public bool IsTransitioning { get { return CurrentTransition != null; } }

		private HashSet<FSMState> _states = new HashSet<FSMState>();
		private Dictionary<FSMState, Dictionary<string, FSMTransition>> _transitions = new Dictionary<FSMState, Dictionary<string, FSMTransition>>();

		private bool isInitialisingState;
		private event Action<FSMState> OnStateEnter;
		private event Action<FSMState> OnStateExit;
		private event Action<FSMState, FSMState> OnStateChange;

		public FSMMachine AddState(FSMState fSMState)
        {
			if (fSMState == null) return this;
			if (_states.Contains(fSMState)) return this;

			_states.Add(fSMState);

			return this;
		}

		public FSMMachine AddTransition(FSMTransition fSMTransition, string command = null)
		{
			if (fSMTransition == null) return this;
			if (!_states.Contains(fSMTransition.FromState)) return this;
			if (!_states.Contains(fSMTransition.ToState)) return this;

			command = string.IsNullOrEmpty(command) ? fSMTransition.ToState.Name : command;

			_transitions[fSMTransition.FromState] = _transitions.ContainsKey(fSMTransition.FromState) ? _transitions[fSMTransition.FromState] : new Dictionary<string, FSMTransition>();
			_transitions[fSMTransition.FromState][command] = fSMTransition;

			return this;
		}

		public void SetDefalutState(FSMState state)
		{
			if (!_states.Contains(state)) return;

			CurrentState = state;
		}

		public bool Transfer(string command)
        {
			if (IsTransitioning) return false;
			if (CurrentState == null) return false;
			if (!_transitions[CurrentState].ContainsKey(command)) return false;

			if (isInitialisingState)
			{
				Debug.LogWarning("Do not call IssueCommand from OnStateChange and OnStateEnter handlers");
				return false;
			}

			var transition = _transitions[CurrentState][command];
			if (transition.TestCondition())
			{
				CurrentTransition = transition;
				transition.OnComplete += HandleTransitionComplete;

                OnStateExit?.Invoke(CurrentState);
                transition.Begin();

				return true;
			}
			return false;
		}

		private void HandleTransitionComplete(FSMTransition transition)
		{
			CurrentTransition.OnComplete -= HandleTransitionComplete;

			PreviousState = CurrentState;
			CurrentState = transition.ToState;

			CurrentTransition = null;
			isInitialisingState = true;

			PreviousState.Exit();
            OnStateChange?.Invoke(PreviousState, CurrentState);

            CurrentState.Enter();
            OnStateEnter?.Invoke(CurrentState);

			isInitialisingState = false;
		}

		public FSMMachine OnEnter(FSMState state, Action handler)
		{
			if (handler == null) { throw new ArgumentNullException("handler"); }
			if (!_states.Contains(state)) { throw new ArgumentException("unknown state", "state"); }

			OnStateEnter += enteredState =>
			{
				if (enteredState.Equals(state))
				{
					handler();
				}
			};

			return this;
		}

		public FSMMachine OnExit(FSMState state, Action handler)
		{
			if (handler == null) { throw new ArgumentNullException("handler"); }
			if (!_states.Contains(state)) { throw new ArgumentException("unknown state", "state"); }

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

	
		public FSMMachine OnChange(FSMState fromState, FSMState toState, Action handler)
		{
			if (!_states.Contains(fromState)) { throw new ArgumentException("unknown state", "from"); }
			if (!_states.Contains(toState)) { throw new ArgumentException("unknown state", "to"); }
			if (handler == null) { throw new ArgumentNullException("handler"); }

			OnStateChange += (from, to) =>
			{
				if (from.Equals(fromState) &&
					to.Equals(toState))
				{
					handler();
				}
			};

			return this;
		}
	}
}
