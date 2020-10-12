using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    
    public class SkillData : ScriptableObject , ISerializationCallbackReceiver
    {
        public int version;
        public SkillItem[] skillItems;
        public SkillCombo skillCombo;

        public void OnAfterDeserialize()
        {
            throw new System.NotImplementedException();
        }

        public void OnBeforeSerialize()
        {
            throw new System.NotImplementedException();
        }
    }

}
