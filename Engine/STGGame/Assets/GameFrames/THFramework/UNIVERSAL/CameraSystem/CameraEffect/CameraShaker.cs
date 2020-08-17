using UnityEngine;

namespace THGame
{
    public class CameraShaker : MonoBehaviour
    {
        // K帧属性
        [Header("震屏(上下、远近、摇头）")]
        public Vector3 shakeArgs = Vector3.zero;
        public float frequency = 1;

        [HideInInspector]public bool shakeable = true;

        // 属性受animation控制，需要在animation之后执行，即使用LateUpdate()
        void LateUpdate()
        {
            if (!shakeable) return;

            ShakeCamera.GetInstance()?.Shake(shakeArgs, frequency);
        }
        private void OnDisable()
        {
            ShakeCamera.GetInstance()?.Recover();
        }

    }
}
