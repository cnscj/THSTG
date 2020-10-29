using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace THGame.Skill
{
    public class SkillEventPlayableAsset : PlayableAsset
    {
        public SkillEvent data;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SkillEventBehaviour : PlayableBehaviour
    {

    }

    [TrackClipType(typeof(SkillEventPlayableAsset))]        //通过这里，将 track 与p layableAsset 绑定联系了起来
    //[TrackBindingType(typeof(GameObject))]
    [TrackColor(0, 1, 0)]
    public class SkillEventPlayableTrack : PlayableTrack
    {

    }


}
