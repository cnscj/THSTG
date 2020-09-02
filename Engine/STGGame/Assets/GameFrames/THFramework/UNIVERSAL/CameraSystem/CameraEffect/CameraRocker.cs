using UnityEngine;

namespace THGame
{
    public class CameraRocker : MonoBehaviour
    {
        // K帧属性
        [Header("抖屏(左右、上下、远近、摇头）")]
        public Vector4 rockArgs = Vector4.zero;

        [HideInInspector]public bool rockable = true;

        // 属性受animation控制，需要在animation之后执行，即使用LateUpdate()
        void Update()
        {
            if (!rockable) return;

            RockCamera.GetInstance()?.Rock(rockArgs);
        }
        private void OnDisable()
        {
            RockCamera.GetInstance()?.Recover();
        }

    }
}
