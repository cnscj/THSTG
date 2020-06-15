using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class ShakeableCamera : MonoSingleton<ShakeableCamera>
    {
        protected class CameraTrans
        {
            public Vector3 localPosition = Vector3.zero;
            public Vector3 localEulerAngles = Vector3.zero;

        }

        protected enum ShakeType
        {
            Args,
            Curve,
        }

        private Vector3 m_shakeArgs = Vector3.right;
        private int m_shakeCount = 10;
        private float m_shakeDuration = 0.2f;

        private AnimationCurve[] m_shakeCurves;    //震屏曲线

        private ShakeType m_type;
        private float m_stayTime = 5f;
        private float m_maxDuration;
        private float m_lastShakeTime;
        private float m_startTick;
        private CameraTrans m_baseTrans = new CameraTrans();
        private CameraTrans m_tempTrans = new CameraTrans();

        public void Shake(CameraShaker shaker)
        {
            if (shaker == null)
                return;

            Shake(shaker.shakeArgs, shaker.shakeDuration, shaker.shakeCount);
        }

        public void Shake(Vector3 args, float duration, int count = 10)
        {
            m_shakeArgs = args;
            m_shakeDuration = duration;
            m_shakeCount = count;
            m_type = ShakeType.Args;

            AwakeSleep();
        }

        public void Shake(AnimationCurve[] curves)
        {
            m_shakeCurves = curves;
            m_type = ShakeType.Curve;

            AwakeSleep();
        }

        public void Stop()
        {
            m_shakeDuration = 0f;
        }

        // 属性受animation控制，需要在animation之后执行，即使用LateUpdate()
        void LateUpdate()
        {
            //休眠时间,自动关闭
            if (CheckSleep())
            {
                Sleep();
                return;
            }

            if (Mathf.Approximately(m_shakeDuration,0))
                return;

            if (m_shakeDuration >= m_lastShakeTime)
                PredoShake();

            ApplyShake();

            m_shakeDuration -= Time.fixedDeltaTime;
            m_shakeDuration = Mathf.Max(0f, m_shakeDuration);
            m_lastShakeTime = m_shakeDuration;

            if (Mathf.Approximately(m_shakeDuration, 0))
                RevertShake();
        }

        void PredoShake()
        {
            m_maxDuration = m_shakeDuration;    //记录最大时长

            RestoreVector();        //还原到起始位置
            SaveVector();           //保存起始位置信息

        }
        
        void ApplyShake()
        {
            //在基础值上变化
            m_tempTrans.localPosition = m_baseTrans.localPosition;
            m_tempTrans.localEulerAngles = m_baseTrans.localEulerAngles;

            //这几个值不变
            m_tempTrans.localPosition.y = transform.localPosition.y;
            m_tempTrans.localEulerAngles.x = transform.localEulerAngles.x;
            m_tempTrans.localEulerAngles.y = transform.localEulerAngles.y;

            switch (m_type)
            {
                case ShakeType.Args:
                    ApplyShakeByArgs();
                    break;
                case ShakeType.Curve:
                    ApplyShakeByCurves();
                    break;
            }

            transform.localPosition = m_tempTrans.localPosition;
            transform.localEulerAngles = m_tempTrans.localEulerAngles;

            UpdateTick();
        }

        //TODO:插值计算
        void ApplyShakeByArgs()
        {
           
            //上下震动插值
            m_tempTrans.localPosition.x += 3;
            //远近震动插值
            m_tempTrans.localPosition.y += 0;
            //摇头震动插值
            m_tempTrans.localEulerAngles.x += 0;

         
        }

        void ApplyShakeByCurves()
        {
            if (m_shakeCurves == null)
                return;
            if (m_shakeCurves.Length <= 0)
                return;

            var shakePosXCurve = m_shakeCurves.Length > 0 ? m_shakeCurves[0] : null;
            var shakePosYCurve = m_shakeCurves.Length > 1 ? m_shakeCurves[1] : null;
            var shakeRotXCurve = m_shakeCurves.Length > 2 ? m_shakeCurves[2] : null;

            float curTime = m_maxDuration - m_shakeDuration;
            if (shakePosXCurve != null)
            {
                var curVal = shakePosXCurve.Evaluate(curTime);
                m_tempTrans.localPosition.x += curVal;
            }
            if (shakePosYCurve != null)
            {
                var curVal = shakePosYCurve.Evaluate(curTime);
                m_tempTrans.localPosition.y += curVal;
            }
            if (shakeRotXCurve != null)
            {
                var curVal = shakeRotXCurve.Evaluate(curTime);
                m_tempTrans.localEulerAngles.x += curVal;
            }
        }


        void RevertShake()
        {
            RestoreVector();        //还原到起始位置
        }

        void SaveVector()
        {
            m_baseTrans.localPosition = transform.localPosition;
            m_baseTrans.localEulerAngles = transform.localEulerAngles;
        }

        void UpdateTick()
        {
            m_startTick = Time.realtimeSinceStartup;
        }

        void AwakeSleep()
        {
            UpdateTick();
            enabled = true;
        }

        void Sleep()
        {
            enabled = false;
        }

        bool CheckSleep()
        {
            if (m_stayTime >= 0f)
            {
                if (m_startTick + m_stayTime <= Time.realtimeSinceStartup)
                {
                    return true;
                }
            }
            return false;
        }

        void RestoreVector()
        {
            transform.localPosition = m_baseTrans.localPosition;
            transform.localEulerAngles = m_baseTrans.localEulerAngles;
        }
    }
}
