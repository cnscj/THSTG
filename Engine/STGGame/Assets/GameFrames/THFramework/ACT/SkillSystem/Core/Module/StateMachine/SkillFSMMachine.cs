using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_WSA
using System.Reflection;
#endif
namespace THGame
{
	public class SkillFSMMachine
	{
		public string Name { get; set; }

		public SkillFSMState PreviousState { get; private set; }
		public SkillFSMState CurrentState { get; private set; }
		public SkillFSMTransition CurrentTransition { get; private set; }

		public bool IsTransitioning { get { return CurrentTransition != null; } }

		private HashSet<SkillFSMState> _states = new HashSet<SkillFSMState>();
		private Dictionary<SkillFSMState, Dictionary<string, SkillFSMTransition>> _transitions = new Dictionary<SkillFSMState, Dictionary<string, SkillFSMTransition>>();

		private bool isInitialisingState;
		private event Action<SkillFSMState> OnStateEnter;
		private event Action<SkillFSMState> OnStateExit;
		private event Action<SkillFSMState, SkillFSMState> OnStateChange;

		public SkillFSMMachine AddState(SkillFSMState fSMState)
		{
			if (fSMState == null) return this;
			if (_states.Contains(fSMState)) return this;

			_states.Add(fSMState);

			return this;
		}

		public SkillFSMMachine AddTransition(SkillFSMTransition fSMTransition, string command = null)
		{
			if (fSMTransition == null) return this;
			if (!_states.Contains(fSMTransition.FromState)) return this;
			if (!_states.Contains(fSMTransition.ToState)) return this;

			command = string.IsNullOrEmpty(command) ? fSMTransition.ToState.Name : command;

			_transitions[fSMTransition.FromState] = _transitions.ContainsKey(fSMTransition.FromState) ? _transitions[fSMTransition.FromState] : new Dictionary<string, SkillFSMTransition>();
			_transitions[fSMTransition.FromState][command] = fSMTransition;

			return this;
		}

		public void DefalutState(SkillFSMState state)
		{
			if (!_states.Contains(state)) return;

			CurrentState = state;
		}

		public bool Transfer(string command)
		{
			if (IsTransitioning) return false;
			if (CurrentState == null)
			{
				return false;
			}
			else
			{
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
				}
				return true;
			}
		}

		private void HandleTransitionComplete(SkillFSMTransition transition)
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

		public SkillFSMMachine OnEnter(SkillFSMState state, Action handler)
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

		public SkillFSMMachine OnExit(SkillFSMState state, Action handler)
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

		public SkillFSMMachine OnChange(Action<SkillFSMState, SkillFSMState> handler)
		{
			if (handler == null) { throw new ArgumentNullException("handler"); }

			OnStateChange += handler;

			return this;
		}


		public SkillFSMMachine OnChange(SkillFSMState fromState, SkillFSMState toState, Action handler)
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
