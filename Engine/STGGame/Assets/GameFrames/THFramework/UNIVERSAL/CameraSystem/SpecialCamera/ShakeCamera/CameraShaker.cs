using UnityEngine;

namespace THGame
{
    public class CameraShaker : MonoBehaviour
    {
        // K帧属性
        [Header("强度(上下,远近,摇头)")] public Vector3 shakeArgs = Vector3.right;
        [Header("持续时间")] public float shakeDuration = 0.2f;

        private void Update()
        {
            if (Mathf.Approximately(shakeDuration, 0f))
                return;

            ShakeableCamera.GetInstance().Shake(this);

            if (shakeDuration > 0)
            {
                shakeDuration -= Time.deltaTime;
                if (shakeDuration <= 0) shakeDuration = 0f;
            }
        }
    }
}
