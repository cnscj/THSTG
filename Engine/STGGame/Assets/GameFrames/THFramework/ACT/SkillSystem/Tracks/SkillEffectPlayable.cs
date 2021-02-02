using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace THGame.Skill
{
    public class SkillEffectPlayableAsset : PlayableAsset
    {
        public string effectId;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SkillEffectBehaviour>.Create(graph);
            var skillEffectBehaviour = playable.GetBehaviour();

            skillEffectBehaviour.effectId = effectId;

            return playable;
        }
    }

    public class SkillEffectBehaviour : PlayableBehaviour
    {
        public string effectId;
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {

        }
    }

    [TrackClipType(typeof(SkillEffectPlayableAsset))]
    [TrackColor(0.3f, 1, 0.4f)]
    public class SkillEffectPlayableTrack : PlayableTrack
    {

    }
}
