using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class AnimatorUtil : MonoBehaviour
    {
        //0没有,1表示出错或中断,2表示动画执行完
        public static int IsCurChipLoopOrOver(Animator animator)
        {
            return IsCurChipLoopOrOverEx(animator, 0f);
        }

        public static int IsCurChipLoopOrOverEx(Animator animator, float exNorTime)
        {
            if (animator != null)
            {
                var clipsInfo = animator.GetCurrentAnimatorClipInfo(0);
                if (clipsInfo != null && clipsInfo.Length > 0)
                {
                    AnimatorClipInfo curClipInfo = clipsInfo[0];
                    var curClip = curClipInfo.clip;
                    if (curClip && curClip.isLooping)
                    {
                        return 1;
                    }
                }

                var curState = animator.GetCurrentAnimatorStateInfo(0);
                if (curState.length <= 0f || curState.loop)
                {
                    return 1;
                }

                if (curState.normalizedTime >= (1.0f + exNorTime))
                {
                    if (exNorTime == 0)
                    {
                        if (animator.IsInTransition(0))
                        {
                            return 0;
                        }
                    }

                    return 2;
                }

                return 0;
            }

            return 1;
        }
    }
}

