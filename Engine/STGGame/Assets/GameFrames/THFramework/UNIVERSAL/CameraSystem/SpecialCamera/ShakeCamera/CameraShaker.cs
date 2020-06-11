using UnityEngine;

namespace THGame
{
    public class CameraShaker : MonoBehaviour
    {
        // K帧属性
        [Header("强度(上下、远近、摇头")] public Vector3 shakeArgs = Vector3.right;
        [Header("震动次数")] public int shakeCount = 10;
        [Header("持续时间")] public float shakeDuration = 0.2f;

        //private void Update()
        //{
        //    if (shakeDuration <= 0)
        //        return;

        //    ShakeableCamera.GetInstance().Shake(this);

        //    shakeDuration -= Time.fixedDeltaTime;
        //    shakeDuration = Mathf.Max(0f, shakeDuration);
        //}
    }
}
