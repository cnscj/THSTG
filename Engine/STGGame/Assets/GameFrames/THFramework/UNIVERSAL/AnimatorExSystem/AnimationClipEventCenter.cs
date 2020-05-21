using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    //管理动画系统的所有事件
    public class AnimationClipEventCenter : MonoSingleton<AnimationClipEventCenter>
    {
        //private HashSet<AnimationClip> m_clipsDict;

        public void AddCompletedEvent(GameObject gameObject, string clipName, AnimationClipEventCallback0 action)
        {
            var clip = GetAnimationClip(gameObject, clipName);
            if (clip != null)
            {
                var callbackComp = GetOrAddCallbackComponent(gameObject);
                AddClipEvent(clip, clip.length, AnimationClipEventCallback.COMPLETED_FUNCNAME);
                callbackComp.onCompleted += action;
            }
        }

        public void AddCustomEvent(GameObject gameObject, string clipName, float time, string tag , AnimationClipEventCallback1 action)
        {
            var clip = GetAnimationClip(gameObject, clipName);
            if (clip != null)
            {
                var callbackComp = GetOrAddCallbackComponent(gameObject);
                var evt = AddClipEvent(clip, time, AnimationClipEventCallback.CUSTOM_FUNCNAME);
                evt.stringParameter = tag;
                callbackComp.onCustom += action;
            }
        }

        public AnimationClip GetAnimationClip(Animator animtor, string clipName)
        {
            if (animtor != null && !string.IsNullOrEmpty(clipName))
            {
                var ctrl = animtor.runtimeAnimatorController;
                if (ctrl != null)
                {
                    var clips = ctrl.animationClips;
                    if (clips != null && clips.Length > 0)
                    {
                        foreach (var clip in clips)
                        {
                            if (clip.name == clipName)
                            {
                                return clip;
                            }
                        }
                    }
                }

            }
            return default;
        }

        public AnimationClip GetAnimationClip(GameObject gameObject, string clipName)
        {
            if (gameObject != null)
            {
                var animator = gameObject.GetComponent<Animator>();
                return GetAnimationClip(animator, clipName);
            }
            return default;
        }
        ///////////////////////
        private AnimationClipEventCallback GetOrAddCallbackComponent(GameObject gameObject)
        {
            if (gameObject != null)
            {
                var callbackComp = gameObject.GetComponent<AnimationClipEventCallback>();
                if (callbackComp == null)
                {
                    callbackComp = gameObject.AddComponent<AnimationClipEventCallback>();
                }
                return callbackComp;
            }
            return default;
        }


        private AnimationEvent AddClipEvent(AnimationClip clip, float time, string funcName)
        {
            if (clip != null && !string.IsNullOrEmpty(funcName))
            {
                var evt = new AnimationEvent();
                evt.time = time;
                evt.functionName = funcName;
                clip.AddEvent(evt);
                return evt;
            }
            return default;
        }

    }
}
