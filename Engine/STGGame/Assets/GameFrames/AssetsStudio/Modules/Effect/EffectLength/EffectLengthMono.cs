﻿using UnityEngine;
using System;
using System.Collections;

namespace ASGame
{
    public class EffectLengthMono : MonoBehaviour
    {
        [Header("时长(-1循环)")]public float length = -2; //-2未知,-1无限,0~Max长度
        public Action onCompleted;
        private WaitForSeconds m_waitForSeconds;

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

                        if (particle.emission.rateOverTimeMultiplier <= 0f)
                        {
                            dunration = particle.main.startDelayMultiplier + particle.main.startLifetimeMultiplier;
                            dunration += particle.main.startLifetimeMultiplier;

                        }
                        else
                        {
                            dunration = particle.main.startDelayMultiplier + Mathf.Max(particle.main.duration, particle.main.startLifetimeMultiplier);
                            dunration += particle.main.startLifetimeMultiplier;
                        }

                        fxTime = Mathf.Max(dunration, fxTime);
                    }
                }
            }
            return fxTime;
        }

        private void OnEnable()
        {
            if (enabled)
            {
                Call();
            }
        }

        private void Call()
        {
            if (length >= 0f)
            {
                m_waitForSeconds = m_waitForSeconds ?? new WaitForSeconds(length);
                StartCoroutine(CountDown());
            }
        }

        private IEnumerator CountDown()
        {
            yield return m_waitForSeconds;
            if (gameObject != null)
            {
                onCompleted?.Invoke();
            }
        }

        public void Calculate()
        {
            length = CalculatePlayEndTime(gameObject);
        }       
    }
}
