using System;
using System.Collections.Generic;
using System.Linq;
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

		private Dictionary<IComparable,SkillFSMState> _states = new Dictionary<IComparable, SkillFSMState>();
		private Dictionary<SkillFSMState, Dictionary<IComparable, SkillFSMTransition>> _transitions = new Dictionary<SkillFSMState, Dictionary<IComparable, SkillFSMTransition>>();

		private bool isInitialisingState;
		private event Action<SkillFSMState> OnStateEnter;
		private event Action<SkillFSMState> OnStateExit;
		private event Action<SkillFSMState, SkillFSMState> OnStateChange;

		public static SkillFSMMachine FromEnum<TState>() where TState : IComparable
		{
			SkillFSMMachine machine = new SkillFSMMachine();
			machine.AddStates<TState>();
			return machine;
		}

		public SkillFSMMachine()
		{
			
		}

		public SkillFSMMachine(SkillFSMState[] fSMStates) : this()
		{
			AddStates(fSMStates);
		}

		public SkillFSMMachine AddStates<TState>() where TState : IComparable
        {
			if (!typeof(Enum).IsAssignableFrom(typeof(TState)))
			{
				throw new Exception("Cannot create finite");
			}

			var states = new List<SkillFSMState>();
			foreach (TState value in Enum.GetValues(typeof(TState)))
			{
				var fsmState = new SkillFSMState(value);
				states.Add(fsmState);
			}
			AddStates(states.ToArray());

			return this;
		}

		public SkillFSMMachine AddStates(SkillFSMState[] fSMStates)
        {
			if (fSMStates == null && fSMStates.Length <= 0)
				return this;

			foreach (var fSMState in fSMStates)
			{
				AddState(fSMState);
			}

			return this;
		}

		public SkillFSMMachine AddState(SkillFSMState fSMState)
		{
			if (fSMState == null) return this;
			if (_states.ContainsValue(fSMState)) return this;

			_states.Add(fSMState.Name, fSMState);
			CurrentState = CurrentState ?? fSMState;

			return this;
		}

		public SkillFSMMachine AddTransition(SkillFSMTransition fSMTransition, IComparable command = null)
		{
			if (fSMTransition == null) return this;
			if (!_states.ContainsValue(fSMTransition.FromState)) return this;
			if (!_states.ContainsValue(fSMTransition.ToState)) return this;

			command = (command == null) ? fSMTransition.ToState.Name : command;

			_transitions[fSMTransition.FromState] = _transitions.ContainsKey(fSMTransition.FromState) ? _transitions[fSMTransition.FromState] : new Dictionary<IComparable, SkillFSMTransition>();
			_transitions[fSMTransition.FromState][command] = fSMTransition;

			return this;
		}

		public SkillFSMMachine AddTransition(IComparable fromState, IComparable toStete, IComparable command , Func<bool> testConditionFunction = null)
        {
			_states.TryGetValue(fromState, out var fromFsmState);
			_states.TryGetValue(toStete, out var toFsmState);

			if (fromFsmState != null && toFsmState != null)
            {
				SkillFSMTransition fSMTransition = new SkillFSMTransition(fromFsmState, toFsmState, testConditionFunction);
				AddTransition(fSMTransition, command);
            }
            else
            {
				Debug.LogWarning("Do not have fromState or toStete");
			}

			return this;
        }

		public SkillFSMMachine AddTransition(IComparable fromState, IComparable toStete, Func<bool> testConditionFunction = null)
		{
			return AddTransition(fromState, toStete, null, testConditionFunction);
		}

		public void Begin(SkillFSMState state)
		{
			if (!_states.ContainsValue(state)) return;

			CurrentState = state;
		}

		public bool IssueCommand(IComparable command)
		{
			if (IsTransitioning) return false;
			if (CurrentState == null)
			{
				return false;
			}
			else
			{
				if (!_transitions.ContainsKey(CurrentState)) return false;
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

		public bool IssueNext()
        {
			if (IsTransitioning) return false;
			if (CurrentState == null)
			{
				return false;
			}
			else
			{
				if (!_transitions.ContainsKey(CurrentState)) return false;

				foreach(var command in _transitions[CurrentState].Keys)
                {
					if (IssueCommand(command))
                    {
						return true;
                    }	
				}
				return false;
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
			if (!_states.ContainsValue(state)) { throw new ArgumentException("unknown state", "state"); }

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
			if (!_states.ContainsValue(state)) { throw new ArgumentException("unknown state", "state"); }

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
			if (!_states.ContainsValue(fromState)) { throw new ArgumentException("unknown state", "from"); }
			if (!_states.ContainsValue(toState)) { throw new ArgumentException("unknown state", "to"); }
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
