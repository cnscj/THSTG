using UnityEngine;
using UnityEngine.Playables;

namespace THGame
{
    public class XXXPlayableAsset : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<XXXPlayableBehavior>.Create(graph);    //通过这里将PlayableAsset 与 PlayableBehaviour 联系在了一起

            return playable;

        }
    }
}