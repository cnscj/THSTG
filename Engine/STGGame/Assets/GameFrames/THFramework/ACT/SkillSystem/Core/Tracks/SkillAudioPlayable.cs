using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace THGame.Skill
{
    public class SkillAudioPlayableAsset : PlayableAsset
    {
        public AudioClip audioClip;
        public SkillAudio data;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SkillAudioBehaviour>.Create(graph);
            var skillAudioBehaviour = playable.GetBehaviour();
            skillAudioBehaviour.audioClip = audioClip;
            return playable;
        }
    }

    public class SkillAudioBehaviour : PlayableBehaviour
    {
        public AudioClip audioClip;
        public bool isFirst;
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            Debug.Log("OnBehaviourPlay");
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!isFirst)
            {
                AudioSource audioSource = playerData as AudioSource;
                if (audioSource != null)
                {
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
                isFirst = true;
            }
            Debug.Log(playerData);
        }
    }

    [TrackClipType(typeof(SkillAudioPlayableAsset))]
    [TrackBindingType(typeof(AudioSource))]
    [TrackColor(0, 0.4f, 0.4f)]
    public class SkillAudioPlayableTrack : PlayableTrack
    {

    }
}
