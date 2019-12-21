using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
namespace THGame
{
    [System.Serializable]
    public class DialogPlayableAsset : PlayableAsset//, ITimelineClipAsset
    {
        public ExposedReference<GameObject> obj;

        // 使其暴露在面板中，但是该类需要序列化
        public DialogPlayableBehaviour np = new DialogPlayableBehaviour();

        //public ClipCaps clipCaps
        //{
        //    get { return ClipCaps.None; }
        //}

        //// Factory method that generates a playable based on this asset
        //public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        //{

        //    np.Obj = obj.Resolve(graph.GetResolver());
        //    return ScriptPlayable<DialogPlayableBehaviour>.Create(graph, np);
        //    //return Playable.Create(graph);
        //}

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<DialogPlayableBehaviour>.Create(graph);    //通过这里将PlayableAsset 与 PlayableBehaviour 联系在了一起

            return playable;

        }

    }

}
