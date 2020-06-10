using UnityEngine;

namespace THGame
{
    //TODO:
    public class CameraShake2 : BaseCameraEffecter
    {
        // K帧属性
        [Header("强度(上下、远近、摇头")] public Vector3 shakeArgs = Vector3.right;
        [Header("震动次数")] public int shakeCount = 10;
        [Header("持续时间")] public float shakeTime = 0.2f;

        private float m_lastShakeTime;
        // 属性受animation控制，需要在animation之后执行，即使用LateUpdate()
        void LateUpdate()
        {
            if (shakeTime <= 0)
                return;

            if (shakeTime >= m_lastShakeTime)
                Predo();
            
            Shake();

            shakeTime -= Time.fixedDeltaTime;
            shakeTime = Mathf.Max(0f, shakeTime);
            m_lastShakeTime = shakeTime;

            if (shakeTime <= 0)
                Restore();
        }

        //插值函数计算
        void Predo()
        {
            Restore();

            Calculate();    //插值函数计算
            Record();       //记录震动开始坐标
        }

        void Calculate()
        {

        }

        //震动取值
        void Shake()
        {
            //进行插值计算
            //Mathf.PerlinNoise
        }

        void Record()
        {
            EffectedCamera.GetInstance().SaveMatrixs();
        }

        void Restore()
        {
            EffectedCamera.GetInstance().BackMatrixs();
        }

        void OnDestroy()
        {
            Restore();
        }
    }
}
