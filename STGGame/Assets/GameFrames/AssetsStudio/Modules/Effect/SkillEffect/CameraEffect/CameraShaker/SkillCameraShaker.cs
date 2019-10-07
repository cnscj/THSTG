using System;
using UnityEngine;

namespace ASGame
{
    // 技能特效扩展组件，挂在主角技能特效的prefab根节点上
    public class SkillCameraShaker : MonoBehaviour
    {
        // K帧属性：
        [Header("震屏（上下、远近、摇头）")]
        public float height = 0;
        public float forward = 0;
        public float rotation = 0;

        // 属性受animation控制，需要在animation之后执行，即使用LateUpdate()
        void LateUpdate()
        {
            if (CameraEffectCamera.GetInstance())
            {
                ApplyShake();
            }
        }

        void OnDisable()
        {
            if (CameraEffectCamera.GetInstance())
            {
                RevertShake();
            }
        }

        void ApplyShake()
        {
            if (CameraEffectCamera.GetInstance())
            {
                foreach(var pair in CameraEffectCamera.GetInstance().cameraMap)
                {
                    var cameraTrans = pair.Key;
                    var cameraInfos = pair.Value;

                    cameraTrans.localPosition = cameraInfos.startPosition + new Vector3(0, height, forward);
                    cameraTrans.localEulerAngles = cameraInfos.startEulerAngles + new Vector3(0, 0, rotation);
                }
            }
        }

        void RevertShake()
        {
            if (CameraEffectCamera.GetInstance())
            {
                foreach (var pair in CameraEffectCamera.GetInstance().cameraMap)
                {
                    var cameraTrans = pair.Key;
                    var cameraInfos = pair.Value;

                    cameraTrans.localPosition = cameraInfos.startPosition;
                    cameraTrans.localEulerAngles = cameraInfos.startEulerAngles;
                }
            }
        }

    }
}
