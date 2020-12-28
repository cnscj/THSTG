using System;

namespace THGame
{
    public class SkillComparableCondition<T> : IComparable<SkillComparableCondition<T>> where T : IComparable
    {
        public SkillLogicalOperator logicalOperator;
        public SkillComparisonOperator comparisonOperator;
        public T value;

        public bool Verify(T value)
        {
            bool ret = false;
            switch (comparisonOperator)
            {
                case SkillComparisonOperator.Greater:
                    ret = value.CompareTo(value) > 0;
                    break;
                case SkillComparisonOperator.Less:
                    ret = value.CompareTo(value) < 0;
                    break;
                case SkillComparisonOperator.Equal:
                    ret = value.CompareTo(value) == 0;
                    break;
                case SkillComparisonOperator.Unequal:
                    ret = value.CompareTo(value) != 0;
                    break;
            }
            return ret;
        }

        public int CompareTo(SkillComparableCondition<T> other)
        {
            return value.CompareTo(other.value);
        }
    }
}
