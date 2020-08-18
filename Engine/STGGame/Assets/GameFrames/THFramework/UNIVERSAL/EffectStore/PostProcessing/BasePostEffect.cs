using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public abstract class BasePostEffect : MonoBehaviour
    {
        //Inspector面板上直接拖入
        public Shader shader = null;
        private Material _material = null;
        public Material _Material
        {
            get
            {
                if (_material == null)
                    _material = GenerateMaterial(shader);
                return _material;
            }
        }

        //根据shader创建用于屏幕特效的材质
        protected Material GenerateMaterial(Shader shader)
        {
            if (shader == null)
                return null;
            //需要判断shader是否支持
            if (shader.isSupported == false)
                return null;
            Material material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            if (material)
                return material;
            return null;
        }

        // Called when start
        protected void CheckResources()
        {
            bool isSupported = CheckSupport();

            if (isSupported == false)
            {
                NotSupported();
            }
        }

        // Called in CheckResources to check support on this platform
        protected bool CheckSupport()
        {
            if (SystemInfo.supportsImageEffects == false || SystemInfo.supportsRenderTextures == false)
            {
                Debug.LogWarning("This platform does not support image effects or render textures.");
                return false;
            }

            return true;
        }

        // Called when the platform doesn't support this effect
        protected void NotSupported()
        {
            enabled = false;
        }

        protected void Start()
        {
            CheckResources();
        }

    }

}
