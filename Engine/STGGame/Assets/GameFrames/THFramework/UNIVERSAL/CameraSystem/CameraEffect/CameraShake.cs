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

        [HideInInspector]
        public float stayTime = -1f;
        private float m_startTick;
    
        // 属性受animation控制，需要在animation之后执行，即使用LateUpdate()
        void LateUpdate()
        {
            //访问超时,自动关闭
            if (stayTime >= 0f)
            {
                if (m_startTick + stayTime <= Time.realtimeSinceStartup)
                {
                    enabled = false;
                    return;
                }
            }

            EffectedCamera.GetInstance().TranslateMatrixs(new Vector3(0, shakeArgs.x, shakeArgs.y), new Vector3(0, 0, shakeArgs.z), Vector3.zero);
        }

        void OnEnable()
        {
            EffectedCamera.GetInstance().SaveMatrixs();
        }

        void OnDisable()
        {
            EffectedCamera.GetInstance().BackMatrixs();
        }

    }
}
