
using UnityEngine;
using UnityEditor;

namespace THGame
{
    public static class CameraShakerUtil
    {
        private static float s_deltaTime = 0.0167f;

        public static AnimationCurve CreateSimpleHarmonicCurve(float motionAmpl, float cycleTime = 0.12f, int cycleCount = 6, bool isDamping = false)
        {
            AnimationCurve shakeCurves = new AnimationCurve();
            if (motionAmpl == 0)
            {
                shakeCurves = AnimationCurve.Linear(0, 0, cycleTime * cycleCount, 0);
            }
            else
            {
                float deltaTime = cycleTime / 4;
                float currentTime = 0f;
                int curCycle = 0;
                float totalTime = 0;

                float curShakeAmpl = motionAmpl;
                float startAmpl = 0f;
                float finalAmpl = startAmpl;

                while (curCycle < cycleCount)
                {
                    while (currentTime >= cycleTime)
                    {
                        currentTime -= cycleTime;
                        curCycle++;
                        if (curCycle >= cycleCount)
                        {
                            finalAmpl = startAmpl;
                            break;
                        }

                        if (!isDamping)
                        {
                            if (motionAmpl != 0f)
                                curShakeAmpl = (cycleCount - curCycle) * motionAmpl / cycleCount;
                        }
                    }

                    bool isSample = false;
                    float val = 2 * Mathf.PI * currentTime / cycleTime;
                    if (curCycle < cycleCount)
                    {
                        float offsetScale = Mathf.Sin(val);
                        if (motionAmpl != 0f)
                            finalAmpl = startAmpl + curShakeAmpl * offsetScale;
                    }
                    //开始或结束必须有,此外波峰波谷采样
                    if (totalTime == 0f || Mathf.Cos(val) < 0.0001f || curCycle >= cycleCount)
                    {
                        isSample = true;
                    }

                    //应该在切线方向等于0时采样
                    if (isSample)
                    {
                        float startTime = totalTime;
                        float endTime = totalTime + deltaTime;

                        var keyFrame = new Keyframe(startTime, endTime);
                        keyFrame.value = finalAmpl;
                        shakeCurves.AddKey(keyFrame);
                    }
                    currentTime += deltaTime;
                    totalTime += deltaTime;
                }
            }
            

            return shakeCurves;
        }

        public static AnimationCurve[] CreateShakeCurve(Vector3 posShake, Vector3 angleShake, float cycleTime = 0.12f, int cycleCount = 6, bool isDamping = false)
        {
            AnimationCurve[] shakeCurves = new AnimationCurve[6];
            shakeCurves[0] = CreateSimpleHarmonicCurve(posShake.x, cycleTime, cycleCount, isDamping);
            shakeCurves[1] = CreateSimpleHarmonicCurve(posShake.y, cycleTime, cycleCount, isDamping);
            shakeCurves[2] = CreateSimpleHarmonicCurve(posShake.z, cycleTime, cycleCount, isDamping);
            shakeCurves[3] = CreateSimpleHarmonicCurve(angleShake.x, cycleTime, cycleCount, isDamping);
            shakeCurves[4] = CreateSimpleHarmonicCurve(angleShake.y, cycleTime, cycleCount, isDamping);
            shakeCurves[5] = CreateSimpleHarmonicCurve(angleShake.z, cycleTime, cycleCount, isDamping);

            return shakeCurves;
        }

    }   
}