using UnityEngine;
using Object = UnityEngine.Object;
namespace ASGame
{
    //管理动画系统的所有事件
    public delegate void AnimationClipEventCallback0();
    public delegate void AnimationClipEventCallback1(string tag);
    public delegate void AnimationClipEventCallback1Ex(Object obj);

    public class AnimationClipEventListener : MonoBehaviour
    {
        public static readonly string COMPLETED_FUNCNAME = "OnClipCompleted";
        public static readonly string CUSTOM_FUNCNAME = "OnClipCustom";
        public static readonly string CUSTOMEX_FUNCNAME = "OnClipCustomEx";

        public AnimationClipEventCallback0 onCompleted;
        public AnimationClipEventCallback1 onCustom;
        public AnimationClipEventCallback1Ex onCustomEx;

        ////////////////////////
        public static void AddCompletedEvent(GameObject gameObject, string clipName, AnimationClipEventCallback0 action)
        {
            var animator = GetAnimator(gameObject);
            var clip = GetAnimationClip(animator, clipName);
            if (clip != null)
            {
                var callbackComp = GetOrAddCallbackComponent(gameObject);
                AnimationClipEventCenter.GetInstance().AddClipEvent(clip, clip.length, COMPLETED_FUNCNAME);
                callbackComp.onCompleted += () => {
                    if (clip.isLooping) return;
                    if (animator.IsInTransition(0)) return;
                    action?.Invoke();
                };
            }
        }

        public static void AddCustomEvent(GameObject gameObject, string clipName, float time, string tag, AnimationClipEventCallback1 action)
        {
            var clip = GetAnimationClip(gameObject, clipName);
            if (clip != null)
            {
                var callbackComp = GetOrAddCallbackComponent(gameObject);
                var evt = AnimationClipEventCenter.GetInstance().AddClipEvent(clip, time, CUSTOM_FUNCNAME);
                evt.stringParameter = tag;
                callbackComp.onCustom += action;
            }
        }

        public static void AddCustomEvent(GameObject gameObject, string clipName, float time, Object obj, AnimationClipEventCallback1Ex action)
        {
            var clip = GetAnimationClip(gameObject, clipName);
            if (clip != null)
            {
                var callbackComp = GetOrAddCallbackComponent(gameObject);
                var evt = AnimationClipEventCenter.GetInstance().AddClipEvent(clip, time, CUSTOMEX_FUNCNAME);
                evt.objectReferenceParameter = obj;
                callbackComp.onCustomEx += action;
            }
        }

        private static AnimationClip GetAnimationClip(Animator animtor, string clipName)
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

        private static Animator GetAnimator(GameObject gameObject)
        {
            if (gameObject != null)
            {
                var animator = gameObject.GetComponent<Animator>();
                return animator;
            }
            return default;
        }

        private static AnimationClip GetAnimationClip(GameObject gameObject, string clipName)
        {
            if (gameObject != null)
            {
                var animator = gameObject.GetComponent<Animator>();
                return GetAnimationClip(animator, clipName);
            }
            return default;
        }

        private static AnimationClipEventListener GetOrAddCallbackComponent(GameObject gameObject)
        {
            if (gameObject != null)
            {
                var callbackComp = gameObject.GetComponent<AnimationClipEventListener>();
                if (callbackComp == null)
                {
                    callbackComp = gameObject.AddComponent<AnimationClipEventListener>();
                }
                return callbackComp;
            }
            return default;
        }

        /////////////////////////

        public void Clear()
        {
            onCompleted = null;
            onCustom = null;
            onCustomEx = null;
        }

        private void OnClipCompleted()
        {
            onCompleted?.Invoke();
        }

        private void OnClipCustom(string tag)
        {
            onCustom?.Invoke(tag);
        }

        private void OnClipCustomEx(Object obj)
        {
            onCustomEx?.Invoke(obj);
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}
