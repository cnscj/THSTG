using System;

namespace THGame
{
	public class StateController
	{
		public event Action OnEntered;
		public event Action OnExited;
		
		public void Enter()
		{
			if (OnEntered != null) OnEntered();
		}

		public void Exit()
		{
			if (OnExited != null) OnExited();
		}
	}
}