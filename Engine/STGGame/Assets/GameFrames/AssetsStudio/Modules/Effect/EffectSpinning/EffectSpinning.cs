using UnityEngine;

namespace ASGame
{
    public class EffectSpinning : MonoBehaviour
    {
        [Header("旋转速度")] public Vector3 eulerAxis = Vector3.zero;
        [Header("时长")] public float duration = -1f;

        void Update()
        {
            if (Mathf.Approximately(duration, 0f))
                return;

            transform.localEulerAngles += eulerAxis;

            if (duration > 0)
            {
                duration -= Time.deltaTime;
                if (duration <= 0) duration = 0f;
            }
        }
    }

}
