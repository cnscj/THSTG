using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class ViewAnimator : ViewBaseClass
    {
        public List<Animator> animators;
        public void Add(GameObject go)
        {
            if (go == null)
                return;

            animators = animators ?? new List<Animator>();
            animators.AddRange(go.GetComponentsInChildren<Animator>());
        }

        public void Play(string state)
        {
            if (animators != null)
            {
                foreach(var animator in animators)
                {
                    animator.Play(state);
                }
            }
        }

        public void CrossFace(string state)
        {
            if (animators != null)
            {
                foreach (var animator in animators)
                {
                    animator.CrossFade(state,0f);
                }
            }
        }

        public void SetInteger(string name,int value)
        {
            if (animators != null)
            {
                foreach (var animator in animators)
                {
                    animator.SetInteger(name, value);
                }
            }
        }

        public void SetBool(string name, bool value)
        {
            if (animators != null)
            {
                foreach (var animator in animators)
                {
                    animator.SetBool(name, value);
                }
            }
        }

    }
}