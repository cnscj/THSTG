using System;
using System.Collections;

namespace THGame
{
    [System.Serializable]
    public class SkillTriggableCondition : IComparable<SkillTriggableCondition>
    {
        public SkillComparisonOperator operation;
        public IComparable value;

        public bool Verify(IComparable otherValue)
        {
            bool ret = false;
            switch (operation)
            {
                case SkillComparisonOperator.Greater:
                    ret = otherValue.CompareTo(value) > 0;
                    break;
                case SkillComparisonOperator.Less:
                    ret = otherValue.CompareTo(value) < 0;
                    break;
                case SkillComparisonOperator.Equal:
                    ret = otherValue.CompareTo(value) == 0;
                    break;
                case SkillComparisonOperator.NotEqual:
                    ret = otherValue.CompareTo(value) != 0;
                    break;
            }
            return ret;
        }

        public int CompareTo(SkillTriggableCondition other)
        {
            if (other == this)
                return 0;

            return value.CompareTo(other.value);
        }
    }

}
