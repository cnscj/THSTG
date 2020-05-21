using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{

    //管理动画系统的所有事件
    public delegate void AnimationClipEventCallback0();
    public delegate void AnimationClipEventCallback1(string tag);

    public class AnimationClipEventCallback : MonoBehaviour
    {
        public static readonly string COMPLETED_FUNCNAME = "OnClipCompleted";
        public static readonly string CUSTOM_FUNCNAME = "OnClipCustom";

        public AnimationClipEventCallback0 onCompleted;
        public AnimationClipEventCallback1 onCustom;

        public AnimationClip GetAnimationLength(string clipName)
        {
            var clip = AnimationClipEventCenter.GetInstance().GetAnimationClip(gameObject, clipName);
            return clip;
        }

        public void AddCompletedEvent(string clipName, AnimationClipEventCallback0 action)
        {
            AnimationClipEventCenter.GetInstance().AddCompletedEvent(gameObject, clipName, action);
        }

        public void AddCustomEvent(string clipName, float time, AnimationClipEventCallback1 action)
        {
            AnimationClipEventCenter.GetInstance().AddCustomEvent(gameObject, clipName, time, action);
        }

        public void Clear()
        {
            onCompleted = null;
            onCustom = null;
        }

        /////////////////////////
        private void OnClipCompleted()
        {
            onCompleted?.Invoke();
        }

        private void OnClipCustom(string tag)
        {
            onCompleted?.Invoke(tag);
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}
