using UnityEngine;
using System.Collections;

namespace THGame
{
    [ExecuteInEditMode]
    public class CameraDarken : MonoBehaviour
    {
        public static CameraDarken Instance { get; protected set; }

        public bool sceneColorApply = false;
        public Color sceneColor = Color.white;

        private void Awake()
        {
            Instance = this;
        }

        void LateUpdate()
        {
            if (!sceneColorApply)
                return;

            ApplyDark();
        }

        void OnDisable()
        {
            RevertDark();
        }


        void ApplyDark()
        {
            if (sceneColorApply)
            {
                Shader.SetGlobalColor("_SceneColor", sceneColor);
            }
        }

        void RevertDark()
        {
            if (sceneColorApply)
            {
                Shader.SetGlobalColor("_SceneColor", Color.white);
            }
        }
    }
}