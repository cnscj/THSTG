using System;

namespace THGame
{
    public class SkillFSMState : IComparable<SkillFSMState>,IEquatable<SkillFSMState>
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

        public int CompareTo(SkillFSMState other)
        {
            if (other == this)
                return 0;

            return Name.CompareTo(other.Name);
        }

        public bool Equals(SkillFSMState other)
        {
            if (other == this)
                return true;

            return Name.Equals(other.Name);
        }
    }

}
