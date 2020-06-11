using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class ShakeableCamera : MonoSingleton<ShakeableCamera>
    {
        protected class CameraTrans
        {
            public Vector3 localPosition;
            public Vector3 localEulerAngles;

        }
        private Vector3 m_shakeArgs = Vector3.right;
        private int m_shakeCount = 10;
        private float m_shakeDuration = 0.2f;
        public float stayTime = 5f;

        private float m_lastShakeTime;
        private float m_startTick;
        private CameraTrans m_trans;

        private Vector3 m_tempLocalPosition = Vector3.zero;
        private Vector3 m_tempLocalEulerAngles = Vector3.zero;

        public void Shake(CameraShaker shaker)
        {
            if (shaker == null)
                return;

            Shake(shaker.shakeArgs, shaker.shakeDuration, shaker.shakeCount);
        }

        public void Shake(Vector3 vector3, float duration, int count = 10)
        {
            m_shakeArgs = vector3;
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
                Predo();

            ApplyShake();

            m_shakeDuration -= Time.fixedDeltaTime;
            m_shakeDuration = Mathf.Max(0f, m_shakeDuration);
            m_lastShakeTime = m_shakeDuration;

            if (Mathf.Approximately(m_shakeDuration, 0))
                RevertShake();
        }

        void Predo()
        {
            RestoreVector();        //还原到起始位置
            SaveVector();           //保存起始位置信息
        }

        //TODO:插值计算
        void ApplyShake()
        {
            //这几个值不变
            m_tempLocalPosition.y = transform.localPosition.y;
            m_tempLocalEulerAngles.x = transform.localEulerAngles.x;
            m_tempLocalEulerAngles.y = transform.localEulerAngles.y;

            //上下震动插值
            m_tempLocalPosition.x = 3;
            //远近震动插值
            m_tempLocalPosition.y = 3;
            //摇头震动插值
            m_tempLocalEulerAngles.x = 3;

            transform.localPosition = m_tempLocalPosition;
            transform.localEulerAngles = m_tempLocalEulerAngles;

            UpdateTick();
        }

        void RevertShake()
        {
            RestoreVector();        //还原到起始位置
        }

        void SaveVector()
        {
            m_trans = m_trans ?? new CameraTrans();
            m_trans.localPosition = transform.localPosition;
            m_trans.localEulerAngles = transform.localEulerAngles;
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
            if (stayTime >= 0f)
            {
                if (m_startTick + stayTime <= Time.realtimeSinceStartup)
                {
                    return true;
                }
            }
            return false;
        }

        void RestoreVector()
        {
            if (m_trans != null)
            {
                transform.localPosition = m_trans.localPosition;
                transform.localEulerAngles = m_trans.localEulerAngles;
            }
        }
    }
}
