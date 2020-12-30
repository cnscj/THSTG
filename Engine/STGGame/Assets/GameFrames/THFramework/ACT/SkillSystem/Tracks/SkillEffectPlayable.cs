using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace THGame.Skill
{
    public class SkillEffectPlayableAsset : PlayableAsset
    {
        public ExposedReference<GameObject> receiveGo;
        public GameObject effectGo;
        public SkillEffect data;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SkillEffectBehaviour>.Create(graph);
            var skillEffectBehaviour = playable.GetBehaviour();

            skillEffectBehaviour.receiveGo = receiveGo.Resolve(graph.GetResolver());
            skillEffectBehaviour.effectGo = effectGo;

            return playable;
        }
    }

    public class SkillEffectBehaviour : PlayableBehaviour
    {
        public GameObject effectGo;
        public GameObject receiveGo;
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (effectGo != null && receiveGo != null)
            {
                var effectInstance = Object.Instantiate(effectGo, receiveGo.transform);
            }
        }
    }

    [TrackClipType(typeof(SkillEffectPlayableAsset))]
    [TrackColor(0.3f, 1, 0.4f)]
    public class SkillEffectPlayableTrack : PlayableTrack
    {

    }
}
