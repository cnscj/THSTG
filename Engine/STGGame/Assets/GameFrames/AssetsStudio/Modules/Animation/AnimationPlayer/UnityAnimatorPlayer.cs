using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class UnityAnimatorPlayer : AnimationPlayer
    {
        public Animator[] animators;

        //TODO:
        public override void AddEvent(string stateName, float time, string evtName)
        {
            if (animators == null || animators.Length <= 0)
                return;

            foreach (var animator in animators)
            {
                var ctrl = animator.runtimeAnimatorController;
                var clips = ctrl.animationClips;

                foreach(var clip in clips)
                {
                    if (string.Compare(clip.name, stateName) != 0)
                        continue;

                    var evt = new AnimationEvent();
                    evt.time = time;
                    clip.AddEvent(evt);
                }

            }
        }

        public override void RemoveEvent(string stateName, float time, string evtName)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnCrossFade(string stateName, float normalizedTransitionDuration, float normalizedTimeOffset)
        {
            if (animators == null || animators.Length <= 0)
                return;

            foreach (var animator in animators)
            {
                animator.CrossFade(stateName, normalizedTransitionDuration, -1, normalizedTimeOffset);
            }
        }


        protected override void OnPlay(string stateName,float normalizedTime)
        {
            if (animators == null || animators.Length <= 0)
                return;

            foreach(var animator in animators)
            {
                animator.Play(stateName, -1 , normalizedTime);
            }
        }
    }
}
