using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class UnityAnimatorPlayer : AnimationPlayer
    {
        public Animator[] animators;
        protected override void OnCrossFade()
        {

        }

        protected override void OnPlay(string stateName)
        {
            if (animators == null || animators.Length <= 0)
                return;

            foreach(var animator in animators)
            {
                animator.Play(stateName);
            }
        }
    }
}
