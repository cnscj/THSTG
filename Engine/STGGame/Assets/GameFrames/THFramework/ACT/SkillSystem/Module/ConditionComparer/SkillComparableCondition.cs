using System;

namespace THGame
{
    public class SkillComparableCondition<T> : ISkillComparableCondition<T>, IComparable<SkillComparableCondition<T>> where T : IComparable
    {
        public SkillComparisonOperator comparisonOperator;
        public T value;

        public bool Verify(T otherValue)
        {
            bool ret = false;
            switch (comparisonOperator)
            {
                case SkillComparisonOperator.Greater:
                    ret = otherValue.CompareTo(value) > 0;
                    break;
                case SkillComparisonOperator.GreaterEqual:
                    ret = otherValue.CompareTo(value) >= 0;
                    break;
                case SkillComparisonOperator.Less:
                    ret = otherValue.CompareTo(value) < 0;
                    break;
                case SkillComparisonOperator.LessEqual:
                    ret = otherValue.CompareTo(value) <= 0;
                    break;
                case SkillComparisonOperator.Equal:
                    ret = otherValue.CompareTo(value) == 0;
                    break;
                case SkillComparisonOperator.Unequal:
                    ret = otherValue.CompareTo(value) != 0;
                    break;
            }
            return ret;
        }

        public int CompareTo(SkillComparableCondition<T> other)
        {
            if (other == this)
                return 0;

            return value.CompareTo(other.value);
        }
    }
}
