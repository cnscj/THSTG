using UnityEngine;

namespace ASGame
{
    public class EffectSpinning : MonoBehaviour
    {
        [Header("各轴旋转速度")] public Vector3 rotationSpeed = Vector3.zero;
        [Header("时长(-1循环)")] public float duration = -1f;

        void Update()
        {
            if (Mathf.Approximately(duration, 0f))
                return;

            transform.Rotate(rotationSpeed);

            if (duration > 0)
            {
                duration -= Time.deltaTime;
                if (duration <= 0) duration = 0f;
            }
        }
    }

}
