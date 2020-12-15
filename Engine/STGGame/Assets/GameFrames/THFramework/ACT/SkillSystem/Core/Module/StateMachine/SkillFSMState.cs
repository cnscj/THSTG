using System;

namespace THGame
{
    public class SkillFSMState
    {
        public string Name { get; private set; }

        public event Action OnEntered;
        public event Action OnExited;

        public SkillFSMState(IComparable comparable):this(comparable.ToString())
        {

        }
        public SkillFSMState(string name)
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
