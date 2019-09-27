using System;
using UnityEngine;

namespace GYGame
{
    // 技能特效扩展组件，挂在主角技能特效的prefab根节点上
    public class SkillCameraShaker : MonoBehaviour
    {
        // K帧属性：
        [Header("震屏（上下、远近、摇头）")]
        public float height = 0;
        public float forward = 0;
        public float rotation = 0;

        // 摄像机对象组，在游戏启动时初始化一次
        static Transform s_sceneCameraShakerTransform;
        static Transform s_sceneCameraHolderTransform;
        static Vector3 s_originLocalEulerAngles;
        static Vector3 s_originLocalPosition;

        // 只有一个生效，最后激活的顶掉之前的
        static SkillCameraShaker s_current;

        public static void InitShake(Transform shakerTransform, Transform holderTransform, bool forceInit = false)
        {
            if (s_sceneCameraShakerTransform != shakerTransform || forceInit)
            {
                s_sceneCameraShakerTransform = shakerTransform;
                s_originLocalEulerAngles = shakerTransform.localEulerAngles;
            }
            if (s_sceneCameraHolderTransform != holderTransform || forceInit)
            {
                s_sceneCameraHolderTransform = holderTransform;
                s_originLocalPosition = holderTransform.localPosition;
            }
        }

#if UNITY_EDITOR
        void Start()
        {
            // 美术编辑器环境下的初始化
            var goShaker = GameObject.Find("SceneCameraShaker");
            var goHolder = GameObject.Find("SceneCameraHolder");
            Debug.Assert(goShaker, "object not found: SceneCameraShaker");
            Debug.Assert(goHolder, "object not found: SceneCameraHolder");
            InitShake(goShaker.transform, goHolder.transform);
        }
#endif

        void OnEnable()
        {
            s_current = this;
        }

        // 属性受animation控制，需要在animation之后执行，即使用LateUpdate()
        void LateUpdate()
        {
            if (s_current != this)
            {
                return;
            }

            ApplyShake();
        }

        void OnDisable()
        {
            if (s_current != this)
            {
                return;
            }
            s_current = null;

            RevertShake();
        }

        void ApplyShake()
        {
            if (s_sceneCameraHolderTransform)
            {
                s_sceneCameraHolderTransform.localPosition = s_originLocalPosition + new Vector3(0, height, forward);
            }
            if (s_sceneCameraShakerTransform)
            {
                s_sceneCameraShakerTransform.localEulerAngles = s_originLocalEulerAngles + new Vector3(0, 0, rotation);
            }
        }

        void RevertShake()
        {
            if (s_sceneCameraHolderTransform)
            {
                s_sceneCameraHolderTransform.localPosition = s_originLocalPosition;
            }
            if (s_sceneCameraShakerTransform)
            {
                s_sceneCameraShakerTransform.localEulerAngles = s_originLocalEulerAngles;
            }
        }

    }
}
