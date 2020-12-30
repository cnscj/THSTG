using System;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillConditionComparer<T1,T2> where T1 : ISkillComparableCondition<T2> where T2 : IComparable
    {
        [SerializeField] public List<T1> conditions;

        public void AddCondition(T1 condition)
        {
            var list = GetConditions();
            list.Add(condition);
        }

        public void AddConditions(T1[] conditions)
        {
            if (conditions == null || conditions.Length <= 0)
                return;

            foreach(var condition in conditions)
            {
                AddCondition(condition);
            }
        }

        public bool Verify(T2 val)
        {
            if (conditions == null || conditions.Count <= 0)
                return false;

            bool finalRet = true;
            foreach (var condition in conditions)
            {
                var logicIoerator = SkillLogicalOperator.And;   //
                var ret = condition.Verify(val);
                switch(logicIoerator)
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

        private List<T1> GetConditions()
        {
            conditions = conditions ?? new List<T1>();
            return conditions;
        }
    }

}
