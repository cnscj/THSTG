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
        private Vector3 m_shakeArgs = Vector3.right;
        private int m_shakeCount = 10;
        private float m_shakeDuration = 0.2f;
        private float m_stayTime = 5f;

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

            AwakeSleep();
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
            RestoreVector();        //还原到起始位置
            SaveVector();           //保存起始位置信息



        }

        //TODO:插值计算
        void ApplyShake()
        {
            //在基础值上变化
            m_tempTrans.localPosition = m_baseTrans.localPosition;
            m_tempTrans.localEulerAngles = m_baseTrans.localEulerAngles;

            //这几个值不变
            m_tempTrans.localPosition.y = transform.localPosition.y;
            m_tempTrans.localEulerAngles.x = transform.localEulerAngles.x;
            m_tempTrans.localEulerAngles.y = transform.localEulerAngles.y;

            //上下震动插值
            m_tempTrans.localPosition.x += 3;
            //远近震动插值
            m_tempTrans.localPosition.y += 0;
            //摇头震动插值
            m_tempTrans.localEulerAngles.x += 0;

            transform.localPosition = m_tempTrans.localPosition;
            transform.localEulerAngles = m_tempTrans.localEulerAngles;

            UpdateTick();
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
