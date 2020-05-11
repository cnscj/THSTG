using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace THGame
{
    [TrackColor(0.855f, 0.903f, 0.87f)]
    [TrackClipType(typeof(DialogPlayableAsset))]// 添加的具体资源类型
    public class DialogTrack : TrackAsset
    {

    }
}
