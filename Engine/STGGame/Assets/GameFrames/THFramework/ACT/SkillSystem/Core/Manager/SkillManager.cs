using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Collection;
using XLibrary.Package;
using Object = UnityEngine.Object;

namespace THGame
{



 



    public class SkillManager : MonoSingleton<SkillManager>
    {
        public SkillFactory Factory = new SkillFactory();
        public SkillLoader Loader = new SkillLoader();
        public SkillTrigger Trigger = new SkillTrigger();
        public SkillCache Cache = new SkillCache();

    }
}
