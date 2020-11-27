using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillNumberValidator : MonoBehaviour
    {
        public enum Operator
        {
            Greater,
            Less,
            Equal,
        }

        public class Condition
        {
            public Operator opera;
            public float value;
        }

        public Condition[] conditions;

        public bool Verify(float num)
        {
            if (conditions == null || conditions.Length < 0)
                return false;

            bool finalRet = true;
            foreach (var condition in conditions)
            {
                finalRet &= Calculate(condition, num);
            }

            return finalRet;
        }

        private bool Calculate(Condition condition,float value)
        {
            bool ret = false;
            switch (condition.opera)
            {
                case Operator.Greater:
                    ret = value > condition.value;
                    break;
                case Operator.Less:
                    ret = value < condition.value;
                    break;
                case Operator.Equal:
                    ret = value == condition.value;
                    break;
            }
            return ret; 
        }
    }

}
