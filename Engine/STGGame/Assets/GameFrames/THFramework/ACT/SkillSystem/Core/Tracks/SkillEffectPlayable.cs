using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace THGame.Skill
{
    public class SkillEffectPlayableAsset : PlayableAsset
    {
        public ExposedReference<GameObject> effectGo;
        public ExposedReference<GameObject> receiveGo;
        public SkillEffect data;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SkillEffectBehaviour>.Create(graph);
            var skillEffectBehaviour = playable.GetBehaviour();

            skillEffectBehaviour.effectGo = effectGo.Resolve(graph.GetResolver());
            skillEffectBehaviour.receiveGo = receiveGo.Resolve(graph.GetResolver());

            return playable;
        }
    }

    public class SkillEffectBehaviour : PlayableBehaviour
    {
        public GameObject effectGo;
        public GameObject receiveGo;
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            Debug.Log(effectGo);
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {

        }
    }

    [TrackClipType(typeof(SkillEffectPlayableAsset))]
    [TrackColor(0.3f, 1, 0.4f)]
    public class SkillEffectPlayableTrack : PlayableTrack
    {

    }
}
