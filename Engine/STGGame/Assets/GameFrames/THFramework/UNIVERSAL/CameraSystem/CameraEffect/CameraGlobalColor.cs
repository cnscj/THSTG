using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [ExecuteInEditMode]
    public class CameraGlobalColor : BaseCameraEffecter
    {
        public Color sceneColor = Color.white;
        private void Update()
        {
            Shader.SetGlobalColor("_SceneColor", sceneColor);
        }
    }
}