using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class ShakeCamera : MonoBehaviour
    {
        private static ShakeCamera s_instance;
        public static float FREQ_COEF = 100f;
        public static float AMPL_COEF = 0.1f;

        [HideInInspector]public Vector3 shakeArgs = Vector3.zero;
        public float frequency = 1;
        public float shakeTime = -1;

        private Vector3 m_shakePosition = Vector3.zero;
        private Vector3 m_shakeRotation = Vector3.zero;
        private VectorMatrix m_startMatrix = new VectorMatrix();

        public static ShakeCamera GetInstance()
        {
            return s_instance;
        }

        private void Awake()
        {
            s_instance = this;
        }

        public void Shake(Vector3 args, float freq, float time = -1f)
        {
            enabled = true;

            shakeArgs = args;
            frequency = freq;
            shakeTime = time;
            if (time > 0)
            {
                m_startMatrix.Save(transform);
            }
        }

        public void Recover()
        {
            shakeArgs = Vector3.zero;
            frequency = 1;
            m_startMatrix.Load(transform);
        }

        void Update()
        {
            if (Mathf.Approximately(shakeTime, 0f))
                return;


            m_shakePosition = CacculateBrownVector(shakeArgs.x);
            m_shakePosition.z = CalculateHarmonicValue(shakeArgs.y, frequency);

            m_shakeRotation.z = CalculateHarmonicValue(shakeArgs.z, frequency);

            transform.localPosition = m_startMatrix.localPosition + m_shakePosition;
            transform.localEulerAngles = m_startMatrix.localEulerAngles + m_shakeRotation;

            if (shakeTime > 0)
            {
                shakeTime -= Time.deltaTime;
                if (shakeTime < 0)
                {
                    shakeTime = 0f;
                    m_startMatrix.Load(transform);
                }
            }
                
        }

        void OnEnable()
        {
            m_startMatrix.Save(transform);
            shakeTime = -1f;
        }

        void OnDisable()
        {
            m_startMatrix.Load(transform);
        }

        //简谐运动
        float CalculateHarmonicValue(float amplitude, float frequency)
        {
            var x1 = Mathf.Cos(Time.fixedTime * frequency * FREQ_COEF);
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

