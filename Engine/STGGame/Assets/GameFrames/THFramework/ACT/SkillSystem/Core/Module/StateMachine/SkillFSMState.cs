using System;

namespace THGame
{
    public class SkillFSMState : IComparable
    {
        public IComparable Name { get; private set; }

        public event Action OnEntered;
        public event Action OnExited;

        public SkillFSMState(IComparable comparable)
        {
            Name = comparable;
        }

        public void Enter()
        {
            if (OnEntered != null) OnEntered();
        }

        public void Exit()
        {
            if (OnExited != null) OnExited();
        }


        public int CompareTo(object obj)
        {
            if (obj == this)
                return 0;

            return Name.CompareTo(obj);
        }
    }

}
