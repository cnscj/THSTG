namespace THGame
{
    [System.Serializable]public class SkillIntCondition : SkillComparableCondition<int> { }
    [System.Serializable]public class SkillIntComparer : SkillConditionComparer<SkillIntCondition, int> { };

    [System.Serializable]public class SkillFloatCondition : SkillComparableCondition<float> { }
    [System.Serializable]public class SkillFloatComparer : SkillConditionComparer<SkillFloatCondition, float> { };

    [System.Serializable]public class SkillBoolCondition : SkillComparableCondition<bool> { }
    [System.Serializable]public class SkillBoolComparer : SkillConditionComparer<SkillBoolCondition, bool> { };
}

