
using UnityEngine;
using UnityEngine.Playables;

namespace THGame.Skill
{
    public class SkillTriggerPlayableClip : PlayableAsset
    {
        public string type;
        public string[] args;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SkillTriggerPlayableBehaviour>.Create(graph);
            var skillTriggertBehaviour = playable.GetBehaviour();
            skillTriggertBehaviour.clip = this;
            skillTriggertBehaviour.OnPlayableCreate(playable);

            return playable;
        }
    }
}