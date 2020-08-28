using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class FSMTransition
    {
		private readonly Func<bool> testConditionFunc;

		public event Action OnComplete;

		public FSMTransition(Func<bool> testConditionFunction = null)
		{
			testConditionFunc = testConditionFunction;
		}

		protected void Complete()
		{
			if (OnComplete != null) OnComplete();
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