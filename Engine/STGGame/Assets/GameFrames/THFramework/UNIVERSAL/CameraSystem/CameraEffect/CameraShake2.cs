using UnityEngine;

namespace THGame
{
    // 震屏控制器,挂载在摄像机上,
    //摄像机必须挂载一个空的父节点上,否则位置会被锁死
    public class CameraShake2 : BaseCameraEffecter
    {
        // K帧属性
        [Header("震屏(上下、远近、摇头）")]
        public Vector3 shakeArgs;

        // 属性受animation控制，需要在animation之后执行，即使用LateUpdate()
        void LateUpdate()
        {
           
        }

        void ApplyShake()
        {

        }

        void RevertShake()
        {
        
        }

    }
}
