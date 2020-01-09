using System;
using UnityEngine;

namespace THGame
{
    // 震屏控制器,挂载在摄像机上
    public class CameraShaker : MonoBehaviour
    {
        public static CameraShaker Instance { get; protected set; }

        // K帧属性
        [Header("震屏(上下、远近、摇头）")]
        public Vector3 shakeArgs;

        //作用对象
        public Transform cameraTrans;
        private Vector3 m_startPosition;
        private Vector3 s_startEulerAngles;

        private void Awake()
        {
            Instance = this;
        }

        // 属性受animation控制，需要在animation之后执行，即使用LateUpdate()
        void LateUpdate()
        {
            if (cameraTrans == null)
                return;

            ApplyShake();
        }

        void OnEnable()
        {
            cameraTrans = cameraTrans ?? Camera.main?.transform ?? gameObject?.transform;
            if (cameraTrans != null)
            {
                m_startPosition = cameraTrans.localPosition;
                s_startEulerAngles = cameraTrans.localEulerAngles;
            }
        }

        void OnDisable()
        {
            if (cameraTrans == null)
                return;

            RevertShake();
        }

        void ApplyShake()
        {
            if (cameraTrans == null)
                return;

            cameraTrans.localPosition = m_startPosition + new Vector3(0, shakeArgs.x, shakeArgs.y);
            cameraTrans.localEulerAngles = s_startEulerAngles + new Vector3(0, 0, shakeArgs.z);   
        }

        void RevertShake()
        {
            if (cameraTrans == null)
                return;

            cameraTrans.localPosition = m_startPosition;
            cameraTrans.localEulerAngles = s_startEulerAngles;
        }

    }
}
