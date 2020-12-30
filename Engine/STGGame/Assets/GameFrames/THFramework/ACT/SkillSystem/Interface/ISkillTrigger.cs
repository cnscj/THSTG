using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public interface ISkillTrigger
    {
        void CastBefore(SkillCastType type);
        void Cast(SkillCastType type);         //技能触发
        void CastAfter(SkillCastType type);

        void CastInterrupt();

    }

}
