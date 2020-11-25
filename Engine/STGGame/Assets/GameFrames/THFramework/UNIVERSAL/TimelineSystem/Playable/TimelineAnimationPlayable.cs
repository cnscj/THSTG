using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace THGame
{
    public class TimelineAnimationBehaviour : PlayableBehaviour
    {
        public AnimationClip animationClip;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            
        }
    }

    //////////////////
    public class TimelineAnimationAsset : PlayableAsset
    {
        public AnimationClip animationClip;
        public SkillAction data;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TimelineAnimationBehaviour>.Create(graph);
            var actionBehaviour = playable.GetBehaviour();
            actionBehaviour.animationClip = animationClip;
            return playable;
        }
    }

    [TrackClipType(typeof(TimelineAnimationAsset))]
    [TrackBindingType(typeof(Animator))]
    [TrackColor(0, 1, 1)]
    public class TimelineAnimationTrack : PlayableTrack
    {

    }
}