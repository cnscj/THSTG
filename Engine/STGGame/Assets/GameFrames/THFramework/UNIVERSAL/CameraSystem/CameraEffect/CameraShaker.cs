using UnityEngine;

namespace THGame
{
    [ExecuteInEditMode]
    public class CameraShaker : BaseCameraEffecter
    {
        public static float FREQ_COEF = 100f;
        public static float AMPL_COEF = 0.1f;

        public static Vector3 shakeHorizontal = Vector3.zero;
        public static Vector3 shakeVertical = Vector3.zero;

        // K帧属性
        [Header("震屏(上下、远近、摇头）")]
        public Vector3 shakeArgs = Vector3.zero;
        public float frequency = 1;

        [HideInInspector]public bool shakeable = true;

        // 属性受animation控制，需要在animation之后执行，即使用LateUpdate()
        void Update()
        {
            if (!shakeable) return;

            shakeHorizontal = CacculateBrownVector(shakeArgs.x);
            shakeHorizontal.z = CalculateHarmonicValue(shakeArgs.y);

            shakeVertical.z = CalculateHarmonicValue(shakeArgs.z);
        }
        void LateUpdate()
        {
            EffectedCamera.GetInstance().TranslateMatrixs(shakeHorizontal, shakeVertical, Vector3.zero);
        }

        void OnEnable()
        {
            GetEffectedCamera().SaveMatrixs();
        }

        void OnDisable()
        {
            GetEffectedCamera()?.BackMatrixs();
        }

        //简谐运动
        float CalculateHarmonicValue(float amplitude)
        {
            var x1 = Mathf.Cos(Time.realtimeSinceStartup * frequency * FREQ_COEF);
            var x2 = AMPL_COEF * amplitude;
            var val = x1 * x2;
            var dir = Mathf.Sign(x1);

            if (dir > 0) val = Mathf.Min(val, dir * x2);
            else val = Mathf.Max(val, dir * x2);

            return val;
        }

        //布朗运动
        Vector2 CacculateBrownVector(float amplitude)
        {
            Vector2 ret = Vector2.zero;
            var newAmplitude = AMPL_COEF * amplitude;
            var randomAngle = Random.Range(1, 360) * Mathf.Deg2Rad;
            ret.x = newAmplitude * Mathf.Cos(randomAngle);
            ret.y = newAmplitude * Mathf.Sin(randomAngle);

            return ret;
        }

        //随机两点运动
        float CalculatePointValue(float amplitude)
        {
            return AMPL_COEF * amplitude * (Random.value < 0.5f ? -1f : 1f);
        }
    }
}
