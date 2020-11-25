using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace THGame.Skill
{
    public class SkillActionPlayableAsset : PlayableAsset
    {
        public AnimationClip animationClip;
        public SkillAction data;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SkillActionBehaviour>.Create(graph);
            var skillActionBehaviour = playable.GetBehaviour();
            skillActionBehaviour.animationClip = animationClip;
            return playable;
        }
    }

    public class SkillActionBehaviour : PlayableBehaviour
    {
        public AnimationClip animationClip;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Debug.Log(playerData);
        }
    }

    [TrackClipType(typeof(SkillActionPlayableAsset))]
    [TrackBindingType(typeof(Animator))]
    [TrackColor(0, 1, 1)]
    public class SkillActionPlayableTrack : PlayableTrack
    {

    }
}
