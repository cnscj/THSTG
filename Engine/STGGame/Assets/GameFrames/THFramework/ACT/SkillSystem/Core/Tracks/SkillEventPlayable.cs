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
            return Playable.Create(graph);
        }
    }

    public class SkillEventBehaviour : PlayableBehaviour
    {

    }

    [TrackClipType(typeof(SkillEventPlayableAsset))]
    [TrackColor(0, 1, 0)]
    public class SkillEventPlayableTrack : PlayableTrack
    {

    }


}
