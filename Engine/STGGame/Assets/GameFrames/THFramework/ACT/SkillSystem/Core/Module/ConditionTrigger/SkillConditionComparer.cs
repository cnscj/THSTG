using System;
using System.Collections.Generic;

namespace THGame
{
    public class SkillConditionComparer<T> where T : IComparable
    {
        public List<SkillComparableCondition<T>> conditions;

        public void AddCondition(SkillComparableCondition<T> condition)
        {
            var list = GetConditions();
            list.Add(condition);
        }

        public bool Verify(T val)
        {
            if (conditions == null || conditions.Count <= 0)
                return false;

            bool finalRet = true;
            foreach (var condition in conditions)
            {
                var ret = condition.Verify(val);
                switch(condition.logicalOperator)
                {
                    case SkillLogicalOperator.And:
                        finalRet &= ret;
                        break;
                    case SkillLogicalOperator.Or:
                        finalRet |= ret;
                        break;
                }
            }

            return finalRet;
        }

        private List<SkillComparableCondition<T>> GetConditions()
        {
            conditions = conditions ?? new List<SkillComparableCondition<T>>();
            return conditions;
        }
    }

}
