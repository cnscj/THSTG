using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace ASGame
{
    [RequireComponent(typeof(Animator))]
    public class CustomPlayablePlayer : MonoBehaviour
    {
        private PlayableGraph _graph;
        private AnimationPlayableOutput _output;
        private AnimationMixerPlayable _mixer;

        private AnimationClipPlayable _curclipPlayable;

        public float tranTime = 2;
        private float leftTime;

        void Start()
        {
            _graph = PlayableGraph.Create();
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            _output = AnimationPlayableOutput.Create(_graph, "Animation", GetComponent<Animator>());


            _mixer = AnimationMixerPlayable.Create(_graph, 2);
            _output.SetSourcePlayable(_mixer);


        }

        public void Play(AnimationClip nextClip)
        {
            _curclipPlayable = AnimationClipPlayable.Create(_graph, nextClip);
            _output.SetSourcePlayable(_curclipPlayable);
            _graph.Play();
        }

        public void CrossFade(AnimationClip nextClip)
        {
            //TODO:
            AnimationClipPlayable nextclipPlayable = AnimationClipPlayable.Create(_graph, nextClip);
            _graph.Connect(_curclipPlayable, 0, _mixer, 0);
            _graph.Connect(nextclipPlayable, 0, _mixer, 1);
            _mixer.SetInputWeight(0, 1);
            _mixer.SetInputWeight(1, 0);

        }

        void Update()
        {
            if (leftTime > 0)
            {
                leftTime = Mathf.Clamp(leftTime - Time.deltaTime, 0, 1);
                float weight = leftTime / tranTime;
                _mixer.SetInputWeight(0, weight);
                _mixer.SetInputWeight(1, 1 - weight);
            }
        }

        private void OnDestroy()
        {
            _graph.Destroy();
        }

    }
}
