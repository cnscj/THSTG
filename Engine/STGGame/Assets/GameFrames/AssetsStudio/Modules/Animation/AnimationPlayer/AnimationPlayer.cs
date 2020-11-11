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
        public int Type { get; }

        public void Stop()
        {

        }

        public void Play(string stateName)
        {
            OnPlay(stateName);
        }

        public void CrossFade()
        {
            OnCrossFade();
        }
        ///////////
        protected abstract void OnPlay(string stateName);

        protected abstract void OnCrossFade();
    }
}
