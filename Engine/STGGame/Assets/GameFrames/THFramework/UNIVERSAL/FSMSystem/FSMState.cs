using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class FSMState : IComparable
    {
        public static readonly FSMState AnyState = new FSMState("AnyState");

        public string Name { get; private set; }

        public event Action OnEntered;
        public event Action OnExited;

        public FSMState(string name)
        {
            Name = name;
        }
        public void Enter()
        {
            if (OnEntered != null) OnEntered();
        }

        public void Exit()
        {
            if (OnExited != null) OnExited();
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            if (obj == this)
                return 0;

            return string.Compare(obj.ToString(), Name);
        }
    }
}