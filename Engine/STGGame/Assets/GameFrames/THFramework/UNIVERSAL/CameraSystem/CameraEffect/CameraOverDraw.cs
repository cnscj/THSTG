﻿using UnityEngine;

namespace THGame
{
    [ExecuteInEditMode]
    public class CameraOverDraw : BaseCameraEffecter
    {
        public static CameraOverDraw Instance { get; protected set; }

        public new Camera camera;
        public bool sceneColorApply = false;
        public Color sceneColor = Color.white;

        private void Awake()
        {
            Instance = this;
        }

        void LateUpdate()
        {
            Apply();
        }

        void Start()
        {
            camera = camera ?? Camera.main;
        }

        void OnEnable()
        {
            if (camera)
            {
                camera.SetReplacementShader(Shader.Find("Hidden/TH/PostProcessOverDraw"),"");
            }
        }

        void OnDisable()
        {
            if (camera)
            {
                camera.ResetReplacementShader();
            }
        }


        void Apply()
        {
            if (sceneColorApply)
            {
                Shader.SetGlobalColor("_OverDrawColor", sceneColor);
            }
            else
            {
                Shader.SetGlobalColor("_OverDrawColor", Color.white);
            }
        }
    }
}