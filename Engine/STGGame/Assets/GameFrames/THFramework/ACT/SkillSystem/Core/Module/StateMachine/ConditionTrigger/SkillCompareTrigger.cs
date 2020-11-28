using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillCompareTrigger<T> where T : IComparable<T>
    {
        public enum Operator
        {
            Greater,
            Less,
            Equal,
            Unequal,
        }

        public class Condition
        {
            public Operator opera;
            public T value;
        }

        public Condition[] conditions;

        public bool Verify(T val)
        {
            if (conditions == null || conditions.Length < 0)
                return false;

            bool finalRet = true;
            foreach (var condition in conditions)
            {
                finalRet &= Calculate(condition, val);
            }

            return finalRet;
        }

        private bool Calculate(Condition condition,T value)
        {
            bool ret = false;
            switch (condition.opera)
            {
                case Operator.Greater:
                    ret = value.CompareTo(condition.value) > 0;
                    break;
                case Operator.Less:
                    ret = value.CompareTo(condition.value) < 0;
                    break;
                case Operator.Equal:
                    ret = value.CompareTo(condition.value) == 0;
                    break;
                case Operator.Unequal:
                    ret = value.CompareTo(condition.value) != 0;
                    break;
            }
            return ret; 
        }
    }

}
