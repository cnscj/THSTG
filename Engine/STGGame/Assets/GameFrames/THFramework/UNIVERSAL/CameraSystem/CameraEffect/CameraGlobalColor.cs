using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [ExecuteInEditMode]
    public class CameraGlobalColor : BaseCameraEffecter
    {
        private static Color s_golbalColor = Color.white;

        public static void SetGlobalColor()
        {
            Shader.SetGlobalColor("_SceneColor", s_golbalColor);
        }
        public static Color GetGlobalColor()
        {
            return Shader.GetGlobalColor("_SceneColor");
        }


        public Color sceneColor = Color.white;
        private void Update()
        {
 
        }
    }
}