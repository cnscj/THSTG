using UnityEngine;

namespace THGame
{
    // 震屏控制器,挂载在摄像机上,
    //摄像机必须挂载一个空的父节点上,否则位置会被锁死
    public class CameraShake : BaseCameraEffecter<CameraShake>
    {
        // K帧属性
        [Header("震屏(上下、远近、摇头）")]
        public Vector3 shakeArgs;

        //作用对象
        public Transform cameraTrans;
        public float stayTime = -1f;
        private Vector3 m_startPosition;
        private Vector3 m_startEulerAngles;

        private float m_startTick;
    
        // 属性受animation控制，需要在animation之后执行，即使用LateUpdate()
        void LateUpdate()
        {
            if (cameraTrans == null)
                return;

            //访问超时,自动关闭
            if (stayTime >= 0f)
            {
                if (m_startTick + stayTime <= Time.realtimeSinceStartup)
                {
                    enabled = false;
                    return;
                }
            }


            ApplyShake();
        }

        void OnEnable()
        {
            cameraTrans = cameraTrans ?? Camera.main?.transform ?? gameObject?.transform;
            if (cameraTrans != null)
            {
                m_startPosition = cameraTrans.localPosition;
                m_startEulerAngles = cameraTrans.localEulerAngles;
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
            cameraTrans.localEulerAngles = m_startEulerAngles + new Vector3(0, 0, shakeArgs.z);   
        }

        void RevertShake()
        {
            if (cameraTrans == null)
                return;

            cameraTrans.localPosition = m_startPosition;
            cameraTrans.localEulerAngles = m_startEulerAngles;
        }

    }
}
