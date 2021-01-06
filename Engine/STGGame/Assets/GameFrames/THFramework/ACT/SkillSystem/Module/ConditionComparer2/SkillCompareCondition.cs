using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{

    public class SkillCompareCondition
    {
        public string key;
        public SkillComparisonOperator comparisonOperator;
        public IComparer value;
    }

}
