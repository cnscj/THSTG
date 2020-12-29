using System;

namespace THGame
{
    public interface ISkillComparableCondition<T>
    {
        bool Verify(T value);
    }
}
