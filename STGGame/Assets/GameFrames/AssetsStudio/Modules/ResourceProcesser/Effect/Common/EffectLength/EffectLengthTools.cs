using UnityEngine;
using System.Collections;

namespace ASGame
{
    public static class EffectLengthTools
    {
        public static float CalculatePlayEndTime(GameObject fxGO)
        {
            float fxTime = -2;    //-2未知,-1无限,0~Max
            if (fxGO != null)
            {
                //取所有动画
                foreach (var animator in fxGO.GetComponentsInChildren<Animator>())
                {
                    if (animator.runtimeAnimatorController)
                    {
                        var clips = animator.runtimeAnimatorController.animationClips;
                        foreach (var clip in clips)
                        {
                            if (clip.isLooping)
                            {
                                fxTime = -1;
                                break;
                            }
                            else
                            {
                                fxTime = Mathf.Max(clip.length, fxTime);
                            }


                        }
                    }
                }


                //取所有粒子
                foreach (var particle in fxGO.GetComponentsInChildren<ParticleSystem>())
                {

                    //如果有一个是循环的,就跳出
                    if (particle.main.loop)
                    {
                        fxTime = -1;
                        break;
                    }
                    else
                    {
                        float dunration = 0f;
                        if (particle.emission.rateOverTime.constantMin <= 0)
                        {
                            dunration = particle.main.startDelay.constantMin + particle.main.startLifetime.constantMin;
                            dunration += particle.main.startLifetime.constantMin;
                        }
                        else
                        {
                            dunration = particle.main.startDelay.constantMin + Mathf.Max(particle.main.duration, particle.main.startLifetime.constantMin);
                            dunration += particle.main.startLifetime.constantMin;

                        }

                        fxTime = Mathf.Max(dunration, fxTime);

                    }
                }

            }

            return fxTime;
        }
    }
}

