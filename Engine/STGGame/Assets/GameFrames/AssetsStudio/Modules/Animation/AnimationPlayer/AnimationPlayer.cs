using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    //主要解决播放循环,播放回调,回调事件,等问题,
    //事件绑定动画,但是响应的实体不一定要接收事件(重复绑定
    public abstract class AnimationPlayer : MonoBehaviour
    {
        public float Speed { get; set; }
        public int Type { get; set; }

        public void Stop()
        {

        }

        public void Play(string stateName, float normalizedTime = 0f)
        {
            OnPlay(stateName, normalizedTime);
        }

        public void CrossFade(string stateName, float normalizedTransitionDuration, float normalizedTimeOffset = 0f)
        {
            OnCrossFade(stateName, normalizedTransitionDuration, normalizedTimeOffset);
        }

        public abstract void AddEvent(string stateName, float time, string evtName);
        public abstract void RemoveEvent(string stateName, float time, string evtName);


        ///////////
        protected abstract void OnPlay(string stateName, float normalizedTime);

        protected abstract void OnCrossFade(string stateName, float normalizedTransitionDuration, float normalizedTimeOffset);

        protected virtual void OnEvent(string stateName, float time, string evtName) { }
    }
}
