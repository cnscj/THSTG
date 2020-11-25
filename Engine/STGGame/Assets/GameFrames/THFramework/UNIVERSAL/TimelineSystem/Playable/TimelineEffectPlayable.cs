using System.Collections;
using System.Collections.Generic;
using ASGame;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace THGame
{
    public class TimelineEffectBehaviour : PlayableBehaviour
    {
        public GameObject effectGo;
        public GameObject receiveGo;
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (effectGo != null && receiveGo != null)
            {
                //需要知道EffectGo的类型,如果是模型特效,则,如果是普通特效,则
                var effectInstance = Object.Instantiate(effectGo, receiveGo.transform);
            }
        }
    }

    //////////////////
    public class TimelineEffectAsset : PlayableAsset
    {
        public ExposedReference<GameObject> receiveGo;
        public GameObject effectGo;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TimelineEffectBehaviour>.Create(graph);
            var skillEffectBehaviour = playable.GetBehaviour();

            skillEffectBehaviour.receiveGo = receiveGo.Resolve(graph.GetResolver());
            skillEffectBehaviour.effectGo = effectGo;

            return playable;
        }
    }

    [TrackClipType(typeof(TimelineEffectAsset))]
    [TrackColor(0.3f, 1, 0.4f)]
    public class TimelineEffectTrack : PlayableTrack
    {

    }
}