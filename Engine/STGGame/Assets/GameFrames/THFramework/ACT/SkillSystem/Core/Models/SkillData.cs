using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillData
    {
        public int version;
        public string modelId;
        public SkillItem[] skillItems;
        public SkillCombo skillCombo;

       
    }

}
