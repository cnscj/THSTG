using System;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillTriggableCondition : IComparable<SkillTriggableCondition>
    {
        public SkillComparisonOperator operation;
        public float threshold;

        public bool Verify(IComparable otherValue)
        {
            if (otherValue == null)
                return false;

            bool ret = false;
            switch (operation)
            {
                case SkillComparisonOperator.Greater:
                    ret = otherValue.CompareTo(threshold) > 0;
                    break;
                case SkillComparisonOperator.Less:
                    ret = otherValue.CompareTo(threshold) < 0;
                    break;
                case SkillComparisonOperator.Equal:
                    ret = otherValue.CompareTo(threshold) == 0;
                    break;
                case SkillComparisonOperator.NotEqual:
                    ret = otherValue.CompareTo(threshold) != 0;
                    break;
            }
            return ret;
        }

        public int CompareTo(SkillTriggableCondition other)
        {
            if (other == this)
                return 0;

            return threshold.CompareTo(other.threshold);
        }
    }

}
