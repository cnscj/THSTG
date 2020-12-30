using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace THGame.Skill
{
    public class SkillEventPlayableAsset : PlayableAsset
    {
        public ExposedReference<GameObject> sender; //引用场景对象
        public SkillEvent data;


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SkillEventBehaviour>.Create(graph);
            var skillEventBehaviour = playable.GetBehaviour();
            skillEventBehaviour.sender = sender.Resolve(graph.GetResolver());
            skillEventBehaviour.args1 = data.args1;
            skillEventBehaviour.eventName = data.eventName;

            return playable;
        }
    }

    public class SkillEventBehaviour : PlayableBehaviour
    {
        public object sender;
        public string eventName;
        public string args1;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (string.IsNullOrEmpty(eventName))
                return;

        }
    }

    [TrackClipType(typeof(SkillEventPlayableAsset))]
    [TrackColor(0, 1, 0)]
    public class SkillEventPlayableTrack : PlayableTrack
    {

    }


}
