using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    //GPUSkinning动画上不能添加事件
    public class GPUSkinningPlayer : AnimationPlayer
    {
        protected override void OnCrossFade()
        {

        }

        protected override void OnPlay(string stateName)
        {

        }
    }
}
