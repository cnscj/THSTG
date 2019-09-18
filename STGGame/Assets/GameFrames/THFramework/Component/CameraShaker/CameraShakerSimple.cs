using UnityEngine;

namespace THGame
{
    public class CameraShakerSimple : MonoBehaviour
    {

        public float shakeUpDown;
        public float shakeLeftRight;
        public float shakeHead = 2;


        public float shakePeriod = 0.12f;
        public int shakeCount = 6;

        private bool m_shake;
        public bool shake;

        void Update()
        {
            if (shake != m_shake)
            {
                Shake();
                m_shake = shake;
            }
        }

        void Shake()
        {
            if (CameraShakerManager.GetInstance())
            {
                CameraShakerCurve curve = new CameraShakerCurve();
                AnimationCurve[] calCurves = CameraShakerUtil.CreateShakeCurve(new Vector3(0, shakeUpDown, shakeLeftRight), new Vector3(0, 0, shakeHead), shakePeriod, shakeCount);
                curve.SetCurves(calCurves);
                
                CameraShakerManager.GetInstance().Shake(curve);
            }
        }

    }
}
