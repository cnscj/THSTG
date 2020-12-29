using System;

namespace THGame
{
    public class SkillFSMState : IComparable<SkillFSMState>,IEquatable<SkillFSMState>
    {
        public IComparable Name { get; private set; }
        public event Action OnEntered;
        public event Action OnExited;
        public object Data { get; set; }

        public SkillFSMState(IComparable comparable)
        {
            Name = comparable;
        }

        public void Enter()
        {
            OnEntered?.Invoke();
        }

        public void Exit()
        {
            OnExited?.Invoke();
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
