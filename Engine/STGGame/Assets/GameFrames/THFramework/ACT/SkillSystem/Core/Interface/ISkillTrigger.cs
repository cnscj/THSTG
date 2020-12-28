using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public interface ISkillTrigger
    {
        void ExecuteBefore();
        void Execute();         //技能触发
        void ExecuteAfter();

        void ExecuteInterrupt();

    }

}
