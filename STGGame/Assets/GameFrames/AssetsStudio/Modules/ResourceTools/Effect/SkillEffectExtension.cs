using System;
using UnityEngine;
using XLibrary;

namespace ASGame
{
    // 技能特效扩展组件，挂在主角技能特效的prefab根节点上
    public class SkillEffectExtension : MonoBehaviour
    {
        // K帧属性：
        [Header("震屏（上下、远近、摇头）")]
        public float height = 0;
        public float forward = 0;
        public float rotation = 0;
        [Header("径向模糊（变化，形状，不透明度）")]
        [Range(0, 5)]
        public float radialBlurChange = 1;
        [Range(0.01f, 2)]
        public float radialBlurShape = 1;
        [Range(0, 1)]
        public float radialBlurAlpha = 1;
        [Header("场景压黑")]
        public Color color = Color.white;

        // 摄像机对象组，在游戏启动时初始化一次
        static Transform s_sceneCameraShakerTransform;
        static Transform s_sceneCameraHolderTransform;
        static Vector3 s_originLocalEulerAngles;
        static Vector3 s_originLocalPosition;

        // 径向模糊代理
        static Action<float, float, float> s_applyRadialBlur;

        // 只有一个生效，最后激活的顶掉之前的
        static SkillEffectExtension s_current;

        public static void InitShake(Transform shakerTransform, Transform holderTransform)
        {
            if (s_sceneCameraShakerTransform != shakerTransform)
            {
                s_sceneCameraShakerTransform = shakerTransform;
                s_originLocalEulerAngles = shakerTransform.localEulerAngles;
            }
            if (s_sceneCameraHolderTransform != holderTransform)
            {
                s_sceneCameraHolderTransform = holderTransform;
                s_originLocalPosition = holderTransform.localPosition;
            }
        }

        public static void InitRadialBlur(Action<float, float, float> action)
        {
            s_applyRadialBlur = action;
        }

#if UNITY_EDITOR
        void Awake()
        {
            if (EditorEnvironment.isArtEditor)
            {
                // 美术编辑器环境下的初始化
                var goShaker = GameObject.Find("SceneCameraShaker");
                var goHolder = GameObject.Find("SceneCameraHolder");
                Debug.Assert(goShaker, "object not found: SceneCameraShaker");
                Debug.Assert(goHolder, "object not found: SceneCameraHolder");
                InitShake(goShaker.transform, goHolder.transform);

                s_applyRadialBlur = EffectEditorRadialBlur.ApplyRadialBlur;
            }
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
            ApplyRadialBlur();
            ApplyDark();
        }

        void OnDisable()
        {
            if (s_current != this)
            {
                return;
            }
            s_current = null;

            RevertShake();
            RevertRadialBlur();
            RevertDark();
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

        void ApplyRadialBlur()
        {
            if (s_applyRadialBlur != null)
            {
                s_applyRadialBlur(radialBlurChange / radialBlurShape, radialBlurShape, radialBlurAlpha);
            }
        }

        void RevertRadialBlur()
        {
            if (s_applyRadialBlur != null)
            {
                s_applyRadialBlur(0, 0, 0);
            }
        }

        void ApplyDark()
        {
            Shader.SetGlobalColor("_SceneColor", color);
        }

        void RevertDark()
        {
            Shader.SetGlobalColor("_SceneColor", Color.white);
        }
    }
}
