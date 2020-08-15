using UnityEngine;

namespace THGame
{
    // 震屏控制器,挂载在摄像机上,
    //摄像机必须挂载一个空的父节点上,否则位置会被锁死
    public class CameraShaker : BaseCameraEffecter
    {
        public static float FREQ_COEF = 100f;
        public static float AMPL_COEF = 0.1f;
        // K帧属性
        [Header("震屏(上下、远近、摇头）")]
        public Vector3 shakeArgs = Vector3.zero;
        public float frequency = 1;

        [HideInInspector]public bool shakeable = true;
        private Vector3 m_shakeHorizontal = Vector3.zero;
        private Vector3 m_shakeVertical = Vector3.zero;

        // 属性受animation控制，需要在animation之后执行，即使用LateUpdate()
        void LateUpdate()
        {
            if (!shakeable) return;

            m_shakeHorizontal.y = CalculateShakeValue(shakeArgs.x);
            m_shakeHorizontal.z = CalculateShakeValue(shakeArgs.y);
            m_shakeVertical.z = CalculateShakeValue(shakeArgs.z);

            EffectedCamera.GetInstance().TranslateMatrixs(m_shakeHorizontal, m_shakeVertical, Vector3.zero);
        }

        void OnEnable()
        {
            EffectedCamera.GetInstance().SaveMatrixs();
        }

        void OnDisable()
        {
            EffectedCamera.GetInstance().BackMatrixs();
        }

        float CalculateShakeValue(float amplitude)
        {
            var x1 = Mathf.Cos(Time.realtimeSinceStartup * frequency * FREQ_COEF);
            var x2 = AMPL_COEF * amplitude;
            var val = x1 * x2;
            var dir = Mathf.Sign(x1);

            if (dir > 0) val = Mathf.Min(val, dir * x2);
            else val = Mathf.Max(val, dir * x2);

            return val;
        }

    }
}
