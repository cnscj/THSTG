using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    //GPUSkinning动画上不能添加事件
    public class GPUSkinningPlayer : AnimationPlayer
    {
        public override void AddEvent(string stateName, float time, string evtName)
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveEvent(string stateName, float time, string evtName)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnCrossFade(string stateName, float normalizedTransitionDuration, float normalizedTimeOffset)
        {
            throw new System.NotImplementedException();
        }


        protected override void OnPlay(string stateName, float normalizedTime)
        {
            throw new System.NotImplementedException();
        }
    }
}
