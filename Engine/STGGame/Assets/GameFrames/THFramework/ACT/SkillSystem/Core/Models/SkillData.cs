using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    
    public class SkillData : ScriptableObject
    {
        public Dictionary<string, SkillItem> skillItems;
        public SkillCombo skillCombo;
    }

}
