using UnityEngine;
using UnityEngine.Playables;

namespace THGame
{
    //某一系列技能的集合,每个技能互相独立
    [System.Serializable]
    public class SkillBean : ScriptableObject
    {
        public int skillId;
        public string skillName;
        public SkillInformation skillInformation;
        public SkillActivity skillActivity;
        public PlayableAsset skillPlayableAsset;
    }

}
