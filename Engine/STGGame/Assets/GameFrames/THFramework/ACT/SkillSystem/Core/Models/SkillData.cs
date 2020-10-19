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

        private Dictionary<string, SkillItem> _skillDict;

        public Dictionary<string, SkillItem> GetDict()
        {
            _skillDict = _skillDict ?? new Dictionary<string, SkillItem>();
            return _skillDict;
        }

    }

}
